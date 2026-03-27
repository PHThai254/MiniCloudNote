import 'package:flutter/material.dart';
import 'package:frontend/features/auth/screens/login_screen.dart';
import 'package:frontend/features/auth/services/auth_service.dart';
import 'package:frontend/features/notes/models/note_model.dart';
import 'package:frontend/features/notes/screens/trash_screen.dart';
import 'package:frontend/features/notes/services/note_service.dart';
import 'package:frontend/features/notes/screens/add_note_screen.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart'; // ĐÃ THÊM: Để lưu trạng thái giao diện
import 'package:frontend/main.dart'; // ĐÃ THÊM: Để gọi biến themeNotifier từ main.dart

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  late Future<List<Note>> _notesFuture;
  final NoteService _noteService = NoteService();
  bool _isSearching = false;
  final TextEditingController _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _loadNotes();
  }

  void _loadNotes([String? query]) {
    setState(() {
      _notesFuture = _noteService.getAllNotes(searchQuery: query);
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: _isSearching
            ? TextField(
                controller: _searchController,
                autofocus: true,
                style: const TextStyle(color: Colors.white),
                decoration: const InputDecoration(
                  hintText: 'Tìm kiếm ghi chú...',
                  hintStyle: TextStyle(color: Colors.white70),
                  border: InputBorder.none,
                ),
                onChanged: (value) {
                  _loadNotes(value);
                },
              )
            : const Text(
                'MiniCloudNote',
                style: TextStyle(fontWeight: FontWeight.bold),
              ),
        backgroundColor: Colors.deepPurple,
        foregroundColor: Colors.white,
        elevation: 0,
        actions: [
          // Nút Tìm Kiếm
          IconButton(
            icon: Icon(_isSearching ? Icons.close : Icons.search),
            onPressed: () {
              setState(() {
                if (_isSearching) {
                  _isSearching = false;
                  _searchController.clear();
                  _loadNotes();
                } else {
                  _isSearching = true;
                }
              });
            },
          ),

          // --- ĐÃ THÊM: NÚT ĐỔI DARK MODE ---
          if (!_isSearching)
            ValueListenableBuilder<ThemeMode>(
              valueListenable: themeNotifier,
              builder: (context, currentMode, child) {
                final isDark = currentMode == ThemeMode.dark;
                return IconButton(
                  icon: Icon(isDark ? Icons.light_mode : Icons.dark_mode),
                  tooltip: isDark ? 'Chế độ Sáng' : 'Chế độ Tối',
                  onPressed: () async {
                    // Đảo trạng thái Sáng/Tối
                    final newMode = isDark ? ThemeMode.light : ThemeMode.dark;
                    themeNotifier.value = newMode;

                    // Lưu cấu hình vào két sắt
                    const storage = FlutterSecureStorage();
                    await storage.write(
                      key: 'app_theme',
                      value: newMode == ThemeMode.dark ? 'dark' : 'light',
                    );
                  },
                );
              },
            ),
          // ------------------------------------

          // Nút Thùng Rác
          if (!_isSearching)
            IconButton(
              icon: const Icon(Icons.delete_outline),
              tooltip: 'Thùng rác',
              onPressed: () async {
                await Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const TrashScreen()),
                );
                _loadNotes();
              },
            ),

          // Nút Đăng xuất
          IconButton(
            icon: const Icon(Icons.logout),
            tooltip: 'Đăng xuất',
            onPressed: () async {
              await AuthService().logoutUser();
              if (context.mounted) {
                Navigator.pushAndRemoveUntil(
                  context,
                  MaterialPageRoute(builder: (context) => const LoginScreen()),
                  (route) => false,
                );
              }
            },
          ),
        ],
      ),
      body: FutureBuilder<List<Note>>(
        future: _notesFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(
              child: CircularProgressIndicator(color: Colors.deepPurple),
            );
          }

          if (snapshot.hasError) {
            final errorMsg = snapshot.error.toString();

            if (errorMsg.contains('TOKEN_EXPIRED')) {
              WidgetsBinding.instance.addPostFrameCallback((_) async {
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(
                    content: Text(
                      'Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại!',
                    ),
                    backgroundColor: Colors.orange,
                  ),
                );
                await AuthService().logoutUser();
                if (context.mounted) {
                  Navigator.pushAndRemoveUntil(
                    context,
                    MaterialPageRoute(
                      builder: (context) => const LoginScreen(),
                    ),
                    (route) => false,
                  );
                }
              });
              return const Center(
                child: CircularProgressIndicator(color: Colors.orange),
              );
            }

            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(Icons.error_outline, color: Colors.red, size: 60),
                  const SizedBox(height: 16),
                  Text(
                    'Lỗi: $errorMsg',
                    style: const TextStyle(color: Colors.red),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: _loadNotes,
                    child: const Text('Thử lại'),
                  ),
                ],
              ),
            );
          }

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

          final notes = snapshot.data!;
          return ListView.builder(
            padding: const EdgeInsets.all(16),
            itemCount: notes.length,
            itemBuilder: (context, index) {
              final note = notes[index];
              final List<Color> pastelColors = [
                const Color(0xFFFFF475),
                const Color(0xFFCCFF90),
                const Color(0xFFCBF0F8),
                const Color(0xFFF28B82),
                const Color(0xFFFDCFE8),
                const Color(0xFFE6C9A8),
                const Color(0xFFD7AEFB),
              ];
              final noteColor =
                  pastelColors[note.id.hashCode.abs() % pastelColors.length];

              return Dismissible(
                key: Key(note.id ?? index.toString()),
                direction: DismissDirection.endToStart,
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
                            onPressed: () => Navigator.of(context).pop(false),
                            child: const Text(
                              "Hủy",
                              style: TextStyle(color: Colors.grey),
                            ),
                          ),
                          TextButton(
                            onPressed: () => Navigator.of(context).pop(true),
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
                onDismissed: (direction) async {
                  if (note.id != null) {
                    try {
                      await NoteService().deleteNote(note.id!);
                      if (context.mounted) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text('Đã xóa ghi chú thành công!'),
                          ),
                        );
                      }
                    } catch (e) {
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
                child: Card(
                  color: noteColor,
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
                        color: Colors
                            .black87, // ĐÃ FIX: Ép màu đen để tránh bị chìm vào nền Pastel khi ở chế độ Dark Mode
                      ),
                    ),
                    subtitle: Text(
                      note.content,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        color: Colors.black54,
                      ), // ĐÃ FIX: Ép màu xám đậm
                    ),
                    trailing: const Icon(
                      Icons.chevron_right,
                      color: Colors.black38,
                    ),
                    onTap: () async {
                      final result = await Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => AddNoteScreen(
                            note: note,
                            backgroundColor: noteColor,
                          ),
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
      floatingActionButton: FloatingActionButton(
        onPressed: () async {
          final result = await Navigator.push(
            context,
            MaterialPageRoute(builder: (context) => const AddNoteScreen()),
          );
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
