import 'package:flutter/material.dart';

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
        colorScheme: ColorScheme.fromSeed(
          seedColor: Colors.deepPurple,
        ), // Màu chủ đạo từ Figma
        useMaterial3: true,
      ),
      home: const Scaffold(
        body: Center(
          child: Text(
            'MiniCloudNote - Ngày 60 Sẵn Sàng!',
            style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
          ),
        ),
      ),
    );
  }
}
