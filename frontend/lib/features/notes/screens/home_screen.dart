import 'package:flutter/material.dart';
import 'package:frontend/features/auth/services/auth_service.dart';
import 'package:frontend/features/notes/models/note_model.dart';
import 'package:frontend/features/notes/services/note_service.dart';
import 'package:frontend/features/notes/screens/add_note_screen.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  // Khai báo một biến Future để chứa danh sách ghi chú sắp được tải về
  late Future<List<Note>> _notesFuture;
  final NoteService _noteService = NoteService();

  @override
  void initState() {
    super.initState();
    // Gọi API ngay khi màn hình vừa được khởi tạo
    _loadNotes();
  }

  // Hàm mồi để gọi NoteService
  void _loadNotes() {
    setState(() {
      _notesFuture = _noteService.getAllNotes();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text(
          'MiniCloudNote',
          style: TextStyle(fontWeight: FontWeight.bold),
        ),
        backgroundColor: Colors.deepPurple,
        foregroundColor: Colors.white,
        elevation: 0,
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            tooltip: 'Đăng xuất',
            onPressed: () {
              AuthService().logoutUser(context);
            },
          ),
        ],
      ),
      // --- SỨC MẠNH CỦA FUTUREBUILDER NẰM Ở ĐÂY ---
      body: FutureBuilder<List<Note>>(
        future: _notesFuture,
        builder: (context, snapshot) {
          // Trạng thái 1: Đang chờ dữ liệu tải về (Hiển thị vòng xoay)
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(
              child: CircularProgressIndicator(color: Colors.deepPurple),
            );
          }

          // Trạng thái 2: Lỗi mạng hoặc Token hết hạn (Hiển thị thông báo đỏ)
          if (snapshot.hasError) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(Icons.error_outline, color: Colors.red, size: 60),
                  const SizedBox(height: 16),
                  Text(
                    'Lỗi: ${snapshot.error}',
                    style: const TextStyle(color: Colors.red),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: _loadNotes, // Bấm để thử tải lại
                    child: const Text('Thử lại'),
                  ),
                ],
              ),
            );
          }

          // Trạng thái 3: Tải thành công nhưng danh sách rỗng (Hiển thị đám mây như cũ)
          if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    Icons.cloud_done_rounded,
                    size: 80,
                    color: Colors.deepPurple.shade200,
                  ),
                  const SizedBox(height: 16),
                  const Text(
                    'Chưa có ghi chú nào!',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.w500),
                  ),
                  const SizedBox(height: 8),
                  const Text(
                    'Hãy tạo ghi chú đầu tiên của bạn.',
                    style: TextStyle(color: Colors.grey),
                  ),
                ],
              ),
            );
          }

          // Trạng thái 4: CÓ DỮ LIỆU! Vẽ danh sách ra màn hình
          final notes = snapshot.data!;
          return ListView.builder(
            padding: const EdgeInsets.all(16),
            itemCount: notes.length,
            itemBuilder: (context, index) {
              final note = notes[index];

              // BỌC THẺ CARD BẰNG DISMISSIBLE
              return Dismissible(
                // Mỗi Dismissible bắt buộc phải có một Key duy nhất (Dùng ID của ghi chú)
                key: Key(note.id ?? index.toString()),
                direction: DismissDirection
                    .endToStart, // Chỉ cho phép vuốt từ phải sang trái
                // 1. Giao diện nền màu đỏ lộ ra khi đang vuốt
                background: Container(
                  alignment: Alignment.centerRight,
                  padding: const EdgeInsets.only(right: 20.0),
                  decoration: BoxDecoration(
                    color: Colors.red.shade400,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  margin: const EdgeInsets.only(bottom: 12),
                  child: const Icon(
                    Icons.delete_outline,
                    color: Colors.white,
                    size: 30,
                  ),
                ),

                // 2. Hiện hộp thoại hỏi "Bạn có chắc chắn?" trước khi vuốt bay mất
                confirmDismiss: (direction) async {
                  return await showDialog(
                    context: context,
                    builder: (BuildContext context) {
                      return AlertDialog(
                        title: const Text("Xác nhận xóa"),
                        content: const Text(
                          "Bạn có chắc chắn muốn xóa ghi chú này không?",
                        ),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                        actions: [
                          TextButton(
                            onPressed: () =>
                                Navigator.of(context).pop(false), // Nhấn Hủy
                            child: const Text(
                              "Hủy",
                              style: TextStyle(color: Colors.grey),
                            ),
                          ),
                          TextButton(
                            onPressed: () =>
                                Navigator.of(context).pop(true), // Nhấn Xóa
                            child: const Text(
                              "Xóa",
                              style: TextStyle(
                                color: Colors.red,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                        ],
                      );
                    },
                  );
                },

                // 3. Sự kiện xảy ra SAU KHI người dùng bấm chữ "Xóa" trên hộp thoại
                onDismissed: (direction) async {
                  if (note.id != null) {
                    try {
                      // Gọi API xóa trên Server
                      await NoteService().deleteNote(note.id!);
                      if (context.mounted) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text('Đã xóa ghi chú thành công!'),
                          ),
                        );
                      }
                    } catch (e) {
                      // Nếu lỗi mạng, tải lại danh sách để "phục hồi" cái thẻ vừa bị vuốt mất
                      _loadNotes();
                      if (context.mounted) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Text('Xóa thất bại: $e'),
                            backgroundColor: Colors.red,
                          ),
                        );
                      }
                    }
                  }
                },

                // 4. ĐÂY CHÍNH LÀ CÁI THẺ CARD GIAO DIỆN CŨ CỦA BẠN
                child: Card(
                  elevation: 2,
                  margin: const EdgeInsets.only(bottom: 12),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: ListTile(
                    title: Text(
                      note.title,
                      style: const TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 16,
                      ),
                    ),
                    subtitle: Text(
                      note.content,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                    trailing: const Icon(
                      Icons.chevron_right,
                      color: Colors.grey,
                    ),
                    onTap: () async {
                      // Logic mở màn hình Edit
                      final result = await Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => AddNoteScreen(note: note),
                        ),
                      );
                      if (result == true) _loadNotes();
                    },
                  ),
                ),
              );
            },
          );
        },
      ),
      // Thêm nút Dấu CỘNG (+) to đùng ở góc dưới để mồi cho tính năng Thêm Ghi Chú
      floatingActionButton: FloatingActionButton(
        onPressed: () async {
          // 1. Chuyển sang màn hình Thêm Ghi chú và ĐỢI kết quả trả về
          final result = await Navigator.push(
            context,
            MaterialPageRoute(builder: (context) => const AddNoteScreen()),
          );

          // 2. Nếu AddNoteScreen báo "true" (Đã lưu thành công), thì load lại danh sách!
          if (result == true) {
            _loadNotes();
          }
        },
        backgroundColor: Colors.deepPurple,
        foregroundColor: Colors.white,
        child: const Icon(Icons.add),
      ),
    );
  }
}
