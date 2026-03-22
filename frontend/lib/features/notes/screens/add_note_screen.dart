import 'package:flutter/material.dart';
import 'package:frontend/features/notes/models/note_model.dart';
import 'package:frontend/features/notes/services/note_service.dart';

class AddNoteScreen extends StatefulWidget {
  final Note? note;
  final Color?
  backgroundColor; // THÊM BIẾN NÀY: Để nhận màu từ HomeScreen truyền sang

  const AddNoteScreen({super.key, this.note, this.backgroundColor});

  @override
  State<AddNoteScreen> createState() => _AddNoteScreenState();
}

class _AddNoteScreenState extends State<AddNoteScreen> {
  final TextEditingController _titleController = TextEditingController();
  final TextEditingController _contentController = TextEditingController();
  final NoteService _noteService = NoteService();
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    // Nếu là chế độ Sửa (Edit), điền sẵn dữ liệu cũ vào ô nhập
    if (widget.note != null) {
      _titleController.text = widget.note!.title;
      _contentController.text = widget.note!.content;
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
            const SizedBox(height: 10),

            // Ô NHẬP NỘI DUNG (Mở rộng chiếm hết phần không gian còn lại)
            Expanded(
              child: TextField(
                controller: _contentController,
                maxLines: null, // <--- Cho phép gõ xuống dòng vô hạn
                expands: true, // <--- Kéo giãn ô nhập phủ kín màn hình
                textAlignVertical:
                    TextAlignVertical.top, // Con trỏ chuột bắt đầu từ trên cùng
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
