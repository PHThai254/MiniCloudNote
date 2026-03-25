import 'dart:io';
import 'package:easy_localization/easy_localization.dart';
import 'package:image_picker/image_picker.dart';
import 'package:flutter/material.dart';
import 'package:frontend/features/notes/models/note_model.dart';
import 'package:frontend/features/notes/services/note_service.dart';
import 'package:flutter_markdown/flutter_markdown.dart'; // Dùng để hiển thị Markdown trong phần xem trước (nếu cần)

class AddNoteScreen extends StatefulWidget {
  final Note? note;
  final Color? backgroundColor; //Để nhận màu từ HomeScreen truyền sang

  const AddNoteScreen({super.key, this.note, this.backgroundColor});

  @override
  State<AddNoteScreen> createState() => _AddNoteScreenState();
}

class _AddNoteScreenState extends State<AddNoteScreen> {
  final TextEditingController _titleController = TextEditingController();
  final TextEditingController _contentController = TextEditingController();
  final NoteService _noteService = NoteService();
  bool _isLoading = false;
  bool _isUploadingImage = false; // Biến trạng thái khi đang tải ảnh lên MinIO
  bool _isPreviewMode =
      false; // Biến trạng thái khi đang ở chế độ Xem trước (Markdown)

  // --- HÀM CHỌN ẢNH VÀ TẢI LÊN ---
  Future<void> _pickAndUploadImage() async {
    final picker = ImagePicker();
    // Mở Thư viện ảnh của điện thoại
    final pickedFile = await picker.pickImage(source: ImageSource.gallery);

    if (pickedFile == null) return; // Nếu người dùng bấm Hủy thì thôi

    setState(() => _isUploadingImage = true);

    try {
      File imageFile = File(pickedFile.path);

      // Gọi chuyến xe tải gửi ảnh lên Server (và lấy link về)
      String imageUrl = await _noteService.uploadImage(imageFile);

      // --- CHÈN MARKDOWN VÀO ĐÚNG VỊ TRÍ CON TRỎ CHUỘT ---
      final text = _contentController.text;
      final selection = _contentController.selection;
      final markdownImage = '\n![Hình ảnh]($imageUrl)\n';

      // Nếu con trỏ chuột đang ở một vị trí xác định trong đoạn text
      if (selection.baseOffset >= 0) {
        _contentController.text = text.replaceRange(
          selection.baseOffset,
          selection.extentOffset,
          markdownImage,
        );
        // Đẩy con trỏ chuột ra phía sau bức ảnh vừa chèn
        _contentController.selection = TextSelection.collapsed(
          offset: selection.baseOffset + markdownImage.length,
        );
      } else {
        // Nếu không xác định được con trỏ chuột, cứ nhét bừa xuống cuối bài
        _contentController.text += markdownImage;
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Lỗi tải ảnh: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    } finally {
      if (mounted) {
        setState(() => _isUploadingImage = false);
      }
    }
  }

  @override
  void initState() {
    super.initState();
    // Nếu là chế độ Sửa (Edit), điền sẵn dữ liệu cũ vào ô nhập
    if (widget.note != null) {
      _titleController.text = widget.note!.title;
      _contentController.text = widget.note!.content;
    }
  }

  // --- HÀM FORMAT NGÀY THÁNG ĐA NGÔN NGỮ ---
  String _getFormattedDate() {
    if (widget.note == null || widget.note!.updatedAt == null) {
      return 'note_detail.just_now'.tr(); // Ghi chú mới -> Hiện "Vừa xong"
    }

    final date = widget.note!.updatedAt!.toLocal();

    if (context.locale.languageCode == 'vi') {
      return DateFormat('dd \'tháng\' MM HH:mm').format(date);
    } else {
      return DateFormat('MMM dd, HH:mm').format(date);
    }
  }

  // Hàm Lưu Ghi chú (Gọi Service)
  Future<void> _saveNote() async {
    final title = _titleController.text.trim();
    final content = _contentController.text.trim();

    if (title.isEmpty || content.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng nhập đầy đủ Tiêu đề và Nội dung!'),
        ),
      );
      return;
    }

    setState(() => _isLoading = true);

    try {
      if (widget.note == null) {
        // TẠO MỚI
        await _noteService.createNote(title, content);
      } else {
        // CẬP NHẬT
        await _noteService.updateNote(widget.note!.id!, title, content);
      }

      if (mounted) {
        Navigator.pop(context, true); // Trả về true báo hiệu đã lưu thành công
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Lỗi: $e'), backgroundColor: Colors.red),
        );
      }
    } finally {
      setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    // Nếu có màu được truyền vào (khi Edit) thì dùng màu đó, nếu tạo mới thì mặc định màu trắng
    final bgColor = widget.backgroundColor ?? Colors.white;

    return Scaffold(
      backgroundColor: bgColor, // <--- PHỦ MÀU LÊN TOÀN MÀN HÌNH
      appBar: AppBar(
        backgroundColor: Colors.transparent, // AppBar trong suốt để hòa vào nền
        elevation: 0, // Xóa đổ bóng của AppBar
        iconTheme: const IconThemeData(
          color: Colors.black87,
        ), // Đổi màu nút Back thành đen
        actions: [
          IconButton(
            icon: Icon(
              _isPreviewMode ? Icons.edit : Icons.remove_red_eye,
              size: 28,
              color: Colors.black87,
            ),
            tooltip: _isPreviewMode ? 'Chỉnh sửa' : 'Xem trước',
            onPressed: () {
              setState(() {
                _isPreviewMode = !_isPreviewMode; // Đảo ngược trạng thái
                // Nếu bật chế độ Xem, tự động thu gọn bàn phím xuống cho dễ nhìn
                if (_isPreviewMode) FocusScope.of(context).unfocus();
              });
            },
          ),
          // 1. NÚT CHỌN ẢNH HOẶC VÒNG XOAY TẢI ẢNH
          if (_isUploadingImage)
            const Padding(
              padding: EdgeInsets.symmetric(horizontal: 16.0),
              child: Center(
                child: SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(
                    strokeWidth: 2,
                    color: Colors.black87,
                  ),
                ),
              ),
            )
          else
            IconButton(
              icon: const Icon(
                Icons.image_outlined,
                size: 28,
                color: Colors.black87,
              ),
              tooltip: 'Chèn hình ảnh',
              onPressed: _pickAndUploadImage, // Gọi hàm chọn ảnh
            ),

          // 2. NÚT LƯU GHI CHÚ
          if (_isLoading)
            const Padding(
              padding: EdgeInsets.all(16.0),
              child: SizedBox(
                width: 24,
                height: 24,
                child: CircularProgressIndicator(strokeWidth: 2),
              ),
            )
          else
            IconButton(
              icon: const Icon(
                Icons.check,
                size: 30,
                color: Colors.black87,
              ), // Nút Lưu hình dấu Tích
              tooltip: 'Lưu ghi chú',
              onPressed: _saveNote,
            ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 20.0),
        child: Column(
          children: [
            // Ô NHẬP TIÊU ĐỀ (Chữ to, in đậm, không viền)
            TextField(
              controller: _titleController,
              style: const TextStyle(
                fontSize: 28,
                fontWeight: FontWeight.bold,
                color: Colors.black87,
              ),
              decoration: const InputDecoration(
                hintText: 'Tiêu đề',
                hintStyle: TextStyle(color: Colors.black38),
                border: InputBorder.none, // <--- XÓA ĐƯỜNG GẠCH CHÂN
              ),
            ),
            const SizedBox(height: 5),

            // --- THANH THÔNG TIN (NGÀY + SỐ KÝ TỰ) ---
            Row(
              children: [
                // Ngày chỉnh sửa
                Text(
                  _getFormattedDate(),
                  style: const TextStyle(color: Colors.black54, fontSize: 13),
                ),
                // Dấu gạch dọc ngăn cách
                const Padding(
                  padding: EdgeInsets.symmetric(horizontal: 8.0),
                  child: Text(
                    '|',
                    style: TextStyle(color: Colors.black54, fontSize: 13),
                  ),
                ),
                // Đếm số ký tự Real-time bằng ValueListenableBuilder
                ValueListenableBuilder<TextEditingValue>(
                  valueListenable: _contentController,
                  builder: (context, value, child) {
                    return Text(
                      '${value.text.length} ${'note_detail.chars'.tr()}',
                      style: const TextStyle(
                        color: Colors.black54,
                        fontSize: 13,
                      ),
                    );
                  },
                ),
              ],
            ),
            // ---------------------------------------------------
            const SizedBox(height: 15),

            // Ô NHẬP NỘI DUNG (Hoặc hiển thị Markdown)
            Expanded(
              child: _isPreviewMode
                  ? // NẾU ĐANG BẬT MẮT XEM TRƯỚC -> GỌI THƯ VIỆN MARKDOWN ĐỂ VẼ ẢNH
                    Markdown(
                      data: _contentController.text.isEmpty
                          ? '*Chưa có nội dung...*'
                          : _contentController.text,
                      styleSheet: MarkdownStyleSheet(
                        p: const TextStyle(
                          fontSize: 18,
                          color: Colors.black87,
                          height: 1.5,
                        ),
                      ),
                    )
                  : // NẾU ĐANG CHỈNH SỬA -> HIỂN THỊ Ô TEXTFIELD BÌNH THƯỜNG
                    TextField(
                      controller: _contentController,
                      maxLines: null, // <--- Cho phép gõ xuống dòng vô hạn
                      expands: true, // <--- Kéo giãn ô nhập phủ kín màn hình
                      textAlignVertical: TextAlignVertical
                          .top, // Con trỏ chuột bắt đầu từ trên cùng
                      style: const TextStyle(
                        fontSize: 18,
                        color: Colors.black87,
                        height: 1.5,
                      ),
                      decoration: const InputDecoration(
                        hintText: 'Nhập nội dung ghi chú của bạn...',
                        hintStyle: TextStyle(color: Colors.black38),
                        border: InputBorder.none, // <--- XÓA ĐƯỜNG GẠCH CHÂN
                      ),
                    ),
            ),
          ],
        ),
      ),
    );
  }
}
