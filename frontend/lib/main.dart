import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/features/auth/screens/login_screen.dart';
import 'package:frontend/features/notes/screens/home_screen.dart';

// TẠO BIẾN TOÀN CỤC ĐỂ QUẢN LÝ THEME (Sáng/Tối)
final ValueNotifier<ThemeMode> themeNotifier = ValueNotifier(ThemeMode.light);
void main() async {
  // Bắt buộc phải có dòng này để Flutter khởi tạo các dịch vụ ngầm (như bộ nhớ máy) trước khi chạy UI
  WidgetsFlutterBinding.ensureInitialized();

  // Đánh thức "Trí nhớ siêu phàm" của Easy Localization
  await EasyLocalization.ensureInitialized();

  // MỞ KÉT SẮT KIỂM TRA XEM LẦN TRƯỚC DÙNG THEME GÌ
  const storage = FlutterSecureStorage();
  String? savedTheme = await storage.read(key: 'app_theme');
  if (savedTheme == 'dark') {
    themeNotifier.value =
        ThemeMode.dark; // Nếu trước đó là Dark thì set là Dark
  }
  runApp(
    EasyLocalization(
      supportedLocales: const [Locale('vi', 'VN'), Locale('en', 'US')],
      path: 'assets/translations', // Chỉ đường đến kho từ điển JSON của bạn
      fallbackLocale: const Locale(
        'vi',
        'VN',
      ), // Nếu lỗi, tự động về Tiếng Việt

      saveLocale:
          true, // SHARED PREFERENCES: Tự động nhớ lựa chọn ngôn ngữ của người dùng
      child: const MiniCloudNoteApp(),
    ),
  );
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
    // 3. BAO BỌC APP BẰNG VALUELISTENABLEBUILDER ĐỂ LẮNG NGHE SỰ THAY ĐỔI
    return ValueListenableBuilder<ThemeMode>(
      valueListenable: themeNotifier,
      builder: (context, currentMode, child) {
        return MaterialApp(
          title: 'MiniCloudNote',
          localizationsDelegates: context.localizationDelegates,
          supportedLocales: context.supportedLocales,
          locale: context.locale,
          debugShowCheckedModeBanner: false,

          // 4. CẤU HÌNH GIAO DIỆN SÁNG VÀ TỐI
          themeMode: currentMode, // <--- Nhận lệnh Sáng/Tối từ Notifier
          // Giao diện Sáng (Mặc định)
          theme: ThemeData(
            colorScheme: ColorScheme.fromSeed(
              seedColor: Colors.deepPurple,
              brightness: Brightness.light,
            ),
            useMaterial3: true,
            scaffoldBackgroundColor: Colors.grey[100],
            appBarTheme: AppBarTheme(
              backgroundColor: Colors.grey[100],
              foregroundColor: Colors.black87,
            ),
          ),

          // Giao diện Tối (Dark Mode) - ĐÃ ĐƯỢC NÂNG CẤP ĐỂ SỬA LỖI Ô NHẬP VĂN BẢN
          darkTheme: ThemeData(
            colorScheme: ColorScheme.fromSeed(
              seedColor: Colors.deepPurple,
              brightness: Brightness.dark,
            ),
            useMaterial3: true,
            scaffoldBackgroundColor: const Color(
              0xFF121212,
            ), // Nền đen chuẩn Material cho toàn app
            appBarTheme: const AppBarTheme(
              backgroundColor: Color(0xFF121212),
              foregroundColor: Colors.white,
            ),
            cardColor: const Color(0xFF1E1E1E),
            // ---------------------------------------------------------------------------------
          ),

          home: FutureBuilder<bool>(
            future: _checkLoginStatus(),
            // ... (Phần builder của FutureBuilder giữ nguyên y hệt của bạn)
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.waiting) {
                return const Scaffold(
                  body: Center(
                    child: CircularProgressIndicator(color: Colors.deepPurple),
                  ),
                );
              }
              if (snapshot.hasError || !(snapshot.data ?? false)) {
                return const LoginScreen();
              }
              return const HomeScreen();
            },
          ),
        );
      },
    );
  }
}
