import 'package:flutter/material.dart';
// Import file login_screen.dart mà bạn vừa tạo
import 'package:frontend/features/auth/screens/login_screen.dart';

void main() {
  runApp(const MiniCloudNoteApp());
}

class MiniCloudNoteApp extends StatelessWidget {
  const MiniCloudNoteApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'MiniCloudNote',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
      ),
      // Đổi trang chủ thành LoginScreen()
      home: const LoginScreen(),
    );
  }
}
