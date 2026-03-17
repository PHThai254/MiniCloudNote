import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

// Import file login_screen.dart mà bạn vừa tạo
import 'package:frontend/features/auth/screens/login_screen.dart';
import 'package:frontend/features/notes/screens/home_screen.dart';

void main() {
  runApp(const MiniCloudNoteApp());
}

class MiniCloudNoteApp extends StatelessWidget {
  const MiniCloudNoteApp({super.key});

  // -- Hàm kiểm tra Két sắt ---
  // Mở két sắt xem có chứa 'jwt_token' bên trong không
  Future<bool> _checkLoginStatus() async {
    const storage = FlutterSecureStorage();
    String? token = await storage.read(key: 'jwt_token');

    // Nếu token khác null và không rỗng nghĩa là đã đăng nhập
    return token != null && token.isNotEmpty;
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'MiniCloudNote',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
      ),
      // Dùng FutureBuilder để quyết định "Trạm đến" đầu tiên
      home: FutureBuilder<bool>(
        future: _checkLoginStatus(),
        builder: (context, snapshot) {
          // Trạng thái 1: Đang chờ mở két sắt
          // Hiển thị vòng xoay loading ở chính giữa màn hình
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Scaffold(
              body: Center(
                child: CircularProgressIndicator(color: Colors.deepPurple),
              ),
            );
          }

          // Trạng thái 2: Mở két xong. Kiểm tra dữ liệu
          // Nếu có lỗi lúc đọc két, HOẶC dữ liệu trả về là false (không có token)
          if (snapshot.hasError || !(snapshot.data ?? false)) {
            return const LoginScreen(); // Bắt đi đăng nhập
          }

          // Trạng thái 3: Két có chứa Token -> Khách quen!
          return const HomeScreen(); // Cho vào thẳng nhà
        },
      ),
    );
  }
}
