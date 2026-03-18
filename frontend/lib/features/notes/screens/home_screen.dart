import 'package:flutter/material.dart';
import 'package:frontend/features/auth/services/auth_service.dart';
import 'package:frontend/features/notes/models/note_model.dart';
import 'package:frontend/features/notes/services/note_service.dart';

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
              return Card(
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
                    maxLines: 2, // Chỉ hiện tối đa 2 dòng nội dung
                    overflow: TextOverflow.ellipsis, // Thêm dấu ... nếu dài quá
                  ),
                  trailing: const Icon(Icons.chevron_right, color: Colors.grey),
                  onTap: () {
                    // Tương lai: Bấm vào đây để xem chi tiết ghi chú
                  },
                ),
              );
            },
          );
        },
      ),
      // Thêm nút Dấu CỘNG (+) to đùng ở góc dưới để mồi cho tính năng Thêm Ghi Chú
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          // Tương lai: Mở màn hình thêm ghi chú mới
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Tính năng Thêm ghi chú sẽ làm ở Ngày 64!'),
            ),
          );
        },
        backgroundColor: Colors.deepPurple,
        foregroundColor: Colors.white,
        child: const Icon(Icons.add),
      ),
    );
  }
}
