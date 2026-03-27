import 'dart:io';
import 'package:easy_localization/easy_localization.dart';
import 'package:image_picker/image_picker.dart';
import 'package:flutter/material.dart';
import 'package:frontend/features/notes/models/note_model.dart';
import 'package:frontend/features/notes/services/note_service.dart';
import 'package:flutter_markdown/flutter_markdown.dart';

class AddNoteScreen extends StatefulWidget {
  final Note? note;
  final Color? backgroundColor;

  const AddNoteScreen({super.key, this.note, this.backgroundColor});

  @override
  State<AddNoteScreen> createState() => _AddNoteScreenState();
}

class _AddNoteScreenState extends State<AddNoteScreen> {
  final TextEditingController _titleController = TextEditingController();
  final TextEditingController _contentController = TextEditingController();
  final NoteService _noteService = NoteService();
  bool _isLoading = false;
  bool _isUploadingImage = false;
  bool _isPreviewMode = false;

  // --- HÀM CHỌN ẢNH VÀ TẢI LÊN ---
  Future<void> _pickAndUploadImage() async {
    final picker = ImagePicker();
    final pickedFile = await picker.pickImage(source: ImageSource.gallery);

    if (pickedFile == null) return;

    setState(() => _isUploadingImage = true);

    try {
      File imageFile = File(pickedFile.path);
      String imageUrl = await _noteService.uploadImage(imageFile);

      final text = _contentController.text;
      final selection = _contentController.selection;
      final markdownImage = '\n![Hình ảnh]($imageUrl)\n';

      if (selection.baseOffset >= 0) {
        _contentController.text = text.replaceRange(
          selection.baseOffset,
          selection.extentOffset,
          markdownImage,
        );
        _contentController.selection = TextSelection.collapsed(
          offset: selection.baseOffset + markdownImage.length,
        );
      } else {
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
    if (widget.note != null) {
      _titleController.text = widget.note!.title;
      _contentController.text = widget.note!.content;
    }
  }

  String _getFormattedDate() {
    if (widget.note == null || widget.note!.updatedAt == null) {
      return 'note_detail.just_now'.tr();
    }

    final date = widget.note!.updatedAt!.toLocal();

    if (context.locale.languageCode == 'vi') {
      return DateFormat('dd \'tháng\' MM HH:mm').format(date);
    } else {
      return DateFormat('MMM dd, HH:mm').format(date);
    }
  }

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
        await _noteService.createNote(title, content);
      } else {
        await _noteService.updateNote(widget.note!.id!, title, content);
      }

      if (mounted) {
        Navigator.pop(context, true);
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
    // 1. KIỂM TRA TRẠNG THÁI DARK MODE
    final isDarkMode = Theme.of(context).brightness == Brightness.dark;

    // 2. CHỌN MÀU NỀN TỰ ĐỘNG
    // Nếu chế độ Tối -> Nền đen.
    // Nếu chế độ Sáng -> Ưu tiên màu Pastel truyền vào, nếu không có thì nền trắng
    final bgColor = isDarkMode
        ? Theme.of(context).scaffoldBackgroundColor
        : (widget.backgroundColor ?? Colors.white);

    // 3. CHỌN MÀU CHỮ & ICON TỰ ĐỘNG
    final textColor = isDarkMode ? Colors.white : Colors.black87;
    final hintColor = isDarkMode ? Colors.white30 : Colors.black38;
    final infoColor = isDarkMode ? Colors.white54 : Colors.black54;

    return Scaffold(
      backgroundColor: bgColor, // <--- NỀN ĐÃ TRỞ NÊN THÔNG MINH
      appBar: AppBar(
        backgroundColor: Colors.transparent,
        elevation: 0,
        iconTheme: IconThemeData(
          color: textColor, // Đổi màu icon góc trái
        ),
        actions: [
          IconButton(
            icon: Icon(
              _isPreviewMode ? Icons.edit : Icons.remove_red_eye,
              size: 28,
              color: textColor, // Màu icon con mắt/cây bút
            ),
            tooltip: _isPreviewMode ? 'Chỉnh sửa' : 'Xem trước',
            onPressed: () {
              setState(() {
                _isPreviewMode = !_isPreviewMode;
                if (_isPreviewMode) FocusScope.of(context).unfocus();
              });
            },
          ),
          if (_isUploadingImage)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16.0),
              child: Center(
                child: SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(
                    strokeWidth: 2,
                    color: textColor, // Vòng xoay
                  ),
                ),
              ),
            )
          else
            IconButton(
              icon: Icon(
                Icons.image_outlined,
                size: 28,
                color: textColor, // Màu icon tải ảnh
              ),
              tooltip: 'Chèn hình ảnh',
              onPressed: _pickAndUploadImage,
            ),
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
              icon: Icon(
                Icons.check,
                size: 30,
                color: textColor, // Màu icon dấu tích
              ),
              tooltip: 'Lưu ghi chú',
              onPressed: _saveNote,
            ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 20.0),
        child: Column(
          children: [
            // Ô NHẬP TIÊU ĐỀ
            TextField(
              controller: _titleController,
              style: TextStyle(
                fontSize: 28,
                fontWeight: FontWeight.bold,
                color: textColor, // Màu tiêu đề
              ),
              decoration: InputDecoration(
                hintText: 'Tiêu đề',
                hintStyle: TextStyle(color: hintColor),
                border: InputBorder.none,
              ),
            ),
            const SizedBox(height: 5),

            // --- THANH THÔNG TIN (NGÀY + SỐ KÝ TỰ) ---
            Row(
              children: [
                Text(
                  _getFormattedDate(),
                  style: TextStyle(color: infoColor, fontSize: 13),
                ),
                Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 8.0),
                  child: Text(
                    '|',
                    style: TextStyle(color: infoColor, fontSize: 13),
                  ),
                ),
                ValueListenableBuilder<TextEditingValue>(
                  valueListenable: _contentController,
                  builder: (context, value, child) {
                    return Text(
                      '${value.text.length} ${'note_detail.chars'.tr()}',
                      style: TextStyle(color: infoColor, fontSize: 13),
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
                  ? Markdown(
                      data: _contentController.text.isEmpty
                          ? '*Chưa có nội dung...*'
                          : _contentController.text,
                      styleSheet: MarkdownStyleSheet(
                        p: TextStyle(
                          fontSize: 18,
                          color: textColor, // Đổi màu chữ hiển thị Markdown
                          height: 1.5,
                        ),
                      ),
                    )
                  : TextField(
                      controller: _contentController,
                      maxLines: null,
                      expands: true,
                      textAlignVertical: TextAlignVertical.top,
                      style: TextStyle(
                        fontSize: 18,
                        color: textColor, // Đổi màu chữ khi đang gõ
                        height: 1.5,
                      ),
                      decoration: InputDecoration(
                        hintText: 'Nhập nội dung ghi chú của bạn...',
                        hintStyle: TextStyle(color: hintColor),
                        border: InputBorder.none,
                      ),
                    ),
            ),
          ],
        ),
      ),
    );
  }
}
