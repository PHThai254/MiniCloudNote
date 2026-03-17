import 'package:flutter/material.dart';
import 'package:frontend/features/auth/services/auth_service.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      // THANH TIÊU ĐỀ VÀ NÚT ĐĂNG XUẤT NẰM Ở ĐÂY
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
              // Gọi hàm đăng xuất mà bạn đã viết ở AuthService
              AuthService().logoutUser(context);
            },
          ),
        ],
      ),
      body: Center(
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
              'Chào mừng bạn đến với trang chủ!',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.w500),
            ),
            const SizedBox(height: 8),
            const Text(
              'Các ghi chú của bạn sẽ hiện ở đây.',
              style: TextStyle(color: Colors.grey),
            ),
          ],
        ),
      ),
    );
  }
}
