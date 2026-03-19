import 'package:flutter/material.dart';
import 'package:frontend/features/notes/services/note_service.dart';

class AddNoteScreen extends StatefulWidget {
  const AddNoteScreen({super.key});

  @override
  State<AddNoteScreen> createState() => _AddNoteScreenState();
}

class _AddNoteScreenState extends State<AddNoteScreen> {
  final TextEditingController _titleController = TextEditingController();
  final TextEditingController _contentController = TextEditingController();
  final NoteService _noteService = NoteService();
  bool _isLoading = false;

  Future<void> _saveNote() async {
    // Chặn người dùng lưu ghi chú trống
    if (_titleController.text.trim().isEmpty &&
        _contentController.text.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng nhập nội dung ghi chú!')),
      );
      return;
    }

    setState(() => _isLoading = true);

    try {
      // Gọi đội vận chuyển NoteService
      bool success = await _noteService.createNote(
        _titleController.text.trim(),
        _contentController.text.trim(),
      );

      if (success && mounted) {
        // Lưu thành công! Trở về màn hình trước và gửi kèm tín hiệu "true" (Báo là đã có thay đổi)
        Navigator.pop(context, true);
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(e.toString()), backgroundColor: Colors.red),
        );
      }
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  void dispose() {
    _titleController.dispose();
    _contentController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Colors.transparent,
        elevation: 0,
        iconTheme: const IconThemeData(
          color: Colors.deepPurple,
        ), // Nút Back màu tím
        actions: [
          // Nút LƯU GHI CHÚ
          _isLoading
              ? const Padding(
                  padding: EdgeInsets.only(right: 20.0),
                  child: Center(
                    child: SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    ),
                  ),
                )
              : IconButton(
                  icon: const Icon(
                    Icons.check_circle,
                    size: 30,
                    color: Colors.deepPurple,
                  ),
                  onPressed: _saveNote,
                  tooltip: 'Lưu ghi chú',
                ),
          const SizedBox(width: 8),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 20.0),
        child: Column(
          children: [
            // Ô nhập Tiêu đề
            TextField(
              controller: _titleController,
              textInputAction: TextInputAction
                  .next, // Bấm "Tiếp" trên bàn phím để nhảy xuống ô Nội dung
              textCapitalization: TextCapitalization.sentences,
              style: const TextStyle(fontSize: 26, fontWeight: FontWeight.bold),
              decoration: const InputDecoration(
                hintText: 'Tiêu đề',
                border: InputBorder.none, // Xóa viền cho đẹp
              ),
              maxLines: null, // Cho phép tự xuống dòng nếu tiêu đề dài
            ),
            // Ô nhập Nội dung
            Expanded(
              child: TextField(
                controller: _contentController,
                textCapitalization: TextCapitalization.sentences,
                style: const TextStyle(fontSize: 18, height: 1.5),
                decoration: const InputDecoration(
                  hintText: 'Bắt đầu gõ nội dung ở đây...',
                  border: InputBorder.none,
                ),
                maxLines: null, // Quan trọng: Cho phép gõ văn bản vô hạn dòng
                keyboardType: TextInputType.multiline,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
