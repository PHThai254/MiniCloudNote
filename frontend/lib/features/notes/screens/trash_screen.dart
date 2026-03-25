import 'package:flutter/material.dart';
import 'package:frontend/features/notes/models/note_model.dart';
import 'package:frontend/features/notes/services/note_service.dart';

class TrashScreen extends StatefulWidget {
  const TrashScreen({super.key});

  @override
  State<TrashScreen> createState() => _TrashScreenState();
}

class _TrashScreenState extends State<TrashScreen> {
  late Future<List<Note>> _trashNotesFuture;
  final NoteService _noteService = NoteService();

  @override
  void initState() {
    super.initState();
    _loadTrashNotes();
  }

  void _loadTrashNotes() {
    setState(() {
      _trashNotesFuture = _noteService.getTrashNotes();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey[100],
      appBar: AppBar(
        title: const Text(
          'Thùng Rác',
          style: TextStyle(fontWeight: FontWeight.bold),
        ),
        backgroundColor: Colors.grey[800],
        foregroundColor: Colors.white,
        elevation: 0,
      ),
      body: FutureBuilder<List<Note>>(
        future: _trashNotesFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(
              child: CircularProgressIndicator(color: Colors.grey),
            );
          }

          if (snapshot.hasError) {
            return Center(
              child: Text(
                'Lỗi: ${snapshot.error}',
                style: const TextStyle(color: Colors.red),
              ),
            );
          }

          if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    Icons.delete_outline,
                    size: 80,
                    color: Colors.grey.shade400,
                  ),
                  const SizedBox(height: 16),
                  const Text(
                    'Thùng rác trống',
                    style: TextStyle(fontSize: 18, color: Colors.grey),
                  ),
                ],
              ),
            );
          }

          final notes = snapshot.data!;
          return ListView.builder(
            padding: const EdgeInsets.all(16),
            itemCount: notes.length,
            itemBuilder: (context, index) {
              final note = notes[index];
              return Dismissible(
                key: Key(note.id ?? index.toString()),
                // BẬT TÍNH NĂNG VUỐT 2 CHIỀU
                direction: DismissDirection.horizontal,

                // NỀN BÊN TRÁI (HIỆN RA KHI VUỐT SANG PHẢI) -> PHỤC HỒI
                background: Container(
                  alignment: Alignment.centerLeft,
                  padding: const EdgeInsets.only(left: 20.0),
                  decoration: BoxDecoration(
                    color: Colors.green.shade400,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  margin: const EdgeInsets.only(bottom: 12),
                  child: const Row(
                    children: [
                      Icon(Icons.restore, color: Colors.white, size: 30),
                      SizedBox(width: 8),
                      Text(
                        'Phục hồi',
                        style: TextStyle(
                          color: Colors.white,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                ),

                // NỀN BÊN PHẢI (HIỆN RA KHI VUỐT SANG TRÁI) -> XÓA VĨNH VIỄN
                secondaryBackground: Container(
                  alignment: Alignment.centerRight,
                  padding: const EdgeInsets.only(right: 20.0),
                  decoration: BoxDecoration(
                    color: Colors.red.shade800,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  margin: const EdgeInsets.only(bottom: 12),
                  child: const Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    children: [
                      Text(
                        'Xóa vĩnh viễn',
                        style: TextStyle(
                          color: Colors.white,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      SizedBox(width: 8),
                      Icon(Icons.delete_forever, color: Colors.white, size: 30),
                    ],
                  ),
                ),

                // XÁC NHẬN HÀNH ĐỘNG DỰA VÀO HƯỚNG VUỐT
                confirmDismiss: (direction) async {
                  if (direction == DismissDirection.startToEnd) {
                    // Vuốt sang phải -> Hỏi có muốn phục hồi không?
                    return await _showConfirmDialog(
                      context,
                      'Phục hồi ghi chú',
                      'Bạn muốn đưa ghi chú này về màn hình chính?',
                      'Phục hồi',
                      Colors.green,
                    );
                  } else {
                    // Vuốt sang trái -> Hỏi có muốn xóa vĩnh viễn không?
                    return await _showConfirmDialog(
                      context,
                      'Xóa vĩnh viễn',
                      'Hành động này không thể hoàn tác. Chắc chắn xóa?',
                      'Xóa',
                      Colors.red,
                    );
                  }
                },

                // THỰC THI GỌI API SAU KHI XÁC NHẬN
                onDismissed: (direction) async {
                  if (note.id != null) {
                    try {
                      if (direction == DismissDirection.startToEnd) {
                        await _noteService.restoreNote(note.id!);
                        if (context.mounted) {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(
                              content: Text('Đã phục hồi ghi chú!'),
                              backgroundColor: Colors.green,
                            ),
                          );
                        }
                      } else {
                        await _noteService.hardDeleteNote(note.id!);
                        if (context.mounted) {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(
                              content: Text('Đã xóa vĩnh viễn!'),
                              backgroundColor: Colors.red,
                            ),
                          );
                        }
                      }
                    } catch (e) {
                      _loadTrashNotes(); // Lỗi thì tải lại danh sách
                    }
                  }
                },

                child: Card(
                  color:
                      Colors.grey[300], // Thẻ trong thùng rác có màu xám u ám
                  elevation: 0,
                  margin: const EdgeInsets.only(bottom: 12),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: ListTile(
                    title: Text(
                      note.title,
                      style: const TextStyle(
                        fontWeight: FontWeight.bold,
                        decoration: TextDecoration.lineThrough,
                        color: Colors.grey,
                      ),
                    ),
                    subtitle: Text(
                      note.content,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                ),
              );
            },
          );
        },
      ),
    );
  }

  // Hàm tiện ích tạo Hộp thoại xác nhận nhanh
  Future<bool?> _showConfirmDialog(
    BuildContext context,
    String title,
    String content,
    String btnText,
    Color btnColor,
  ) {
    return showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(title),
        content: Text(content),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Hủy', style: TextStyle(color: Colors.grey)),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            child: Text(
              btnText,
              style: TextStyle(color: btnColor, fontWeight: FontWeight.bold),
            ),
          ),
        ],
      ),
    );
  }
}
