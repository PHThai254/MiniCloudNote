import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:frontend/features/auth/screens/register_screen.dart';
import 'package:frontend/features/auth/services/auth_service.dart';
import 'package:frontend/features/notes/screens/home_screen.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();

  bool _isLoading = false;
  bool _obscurePassword = true;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    // Kỹ thuật bắt trạng thái Theme hiện tại (Sáng hay Tối)
    final isDarkMode = Theme.of(context).brightness == Brightness.dark;

    // Các biến màu sắc tự động thay đổi theo Theme
    final bgColor = isDarkMode
        ? Theme.of(context).scaffoldBackgroundColor
        : Colors.white;
    final textColor = isDarkMode ? Colors.white : Colors.black87;
    final subtitleColor = isDarkMode ? Colors.white70 : Colors.grey;
    final textFieldColor = isDarkMode ? Colors.grey[900] : Colors.grey[50];
    final borderColor = isDarkMode ? Colors.grey[800] : Colors.grey.shade300;

    return Scaffold(
      backgroundColor: bgColor, // <-- Dùng màu linh hoạt
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 40.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const SizedBox(height: 20),
              // --- Header ---
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'login.title'.tr(),
                    style: TextStyle(
                      fontSize: 32,
                      fontWeight: FontWeight.bold,
                      color: textColor, // <-- Màu chữ tự động chỉnh sửa
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.language, color: Colors.deepPurple),
                    onPressed: () {
                      if (context.locale.languageCode == 'vi') {
                        context.setLocale(const Locale('en', 'US'));
                      } else {
                        context.setLocale(const Locale('vi', 'VN'));
                      }
                    },
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Text(
                'login.subtitle'.tr(),
                style: TextStyle(
                  fontSize: 16,
                  color: subtitleColor,
                ), // <-- Đã sửa
              ),
              const SizedBox(height: 50),

              // --- Email Input ---
              Text(
                'login.email_label'.tr(),
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: textColor, // <-- Đã sửa
                ),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: _emailController,
                keyboardType: TextInputType.emailAddress,
                style: TextStyle(color: textColor), // <-- Ép màu chữ khi gõ
                decoration: InputDecoration(
                  hintText: 'login.email_hint'.tr(),
                  hintStyle: TextStyle(
                    color: isDarkMode ? Colors.white30 : Colors.black38,
                  ), // <-- Đã sửa
                  filled: true,
                  fillColor: textFieldColor, // <-- Đã sửa
                  contentPadding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 16,
                  ),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide(color: borderColor!), // <-- Đã sửa
                  ),
                  enabledBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide(color: borderColor), // <-- Đã sửa
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: const BorderSide(
                      color: Colors.deepPurple,
                      width: 2,
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 24),

              // --- Password Input ---
              Text(
                'login.password_label'.tr(),
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: textColor, // <-- Đã sửa
                ),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: _passwordController,
                obscureText: _obscurePassword,
                style: TextStyle(color: textColor), // <-- Ép màu chữ khi gõ
                decoration: InputDecoration(
                  hintText: 'login.password_hint'.tr(),
                  hintStyle: TextStyle(
                    color: isDarkMode ? Colors.white30 : Colors.black38,
                  ), // <-- Đã sửa
                  filled: true,
                  fillColor: textFieldColor, // <-- Đã sửa
                  contentPadding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 16,
                  ),
                  suffixIcon: IconButton(
                    icon: Icon(
                      _obscurePassword
                          ? Icons.visibility_off
                          : Icons.visibility,
                      color: subtitleColor, // <-- Đã sửa
                    ),
                    onPressed: () {
                      setState(() {
                        _obscurePassword = !_obscurePassword;
                      });
                    },
                  ),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide(color: borderColor), // <-- Đã sửa
                  ),
                  enabledBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide(color: borderColor), // <-- Đã sửa
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: const BorderSide(
                      color: Colors.deepPurple,
                      width: 2,
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 12),

              // --- Forgot Password ---
              Align(
                alignment: Alignment.centerLeft,
                child: TextButton(
                  onPressed: () {},
                  style: TextButton.styleFrom(
                    padding: EdgeInsets.zero,
                    minimumSize: const Size(50, 30),
                    tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                  ),
                  child: Text(
                    'login.forgot_password'.tr(),
                    style: const TextStyle(
                      color: Colors.deepPurple,
                      fontWeight: FontWeight.w600,
                      decoration: TextDecoration.underline,
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 40),

              // --- Login Button (Giữ nguyên logic của bạn) ---
              ElevatedButton(
                onPressed: _isLoading
                    ? null
                    : () async {
                        final email = _emailController.text;
                        final password = _passwordController.text;

                        if (email.isEmpty || password.isEmpty) {
                          ScaffoldMessenger.of(context).showSnackBar(
                            SnackBar(
                              content: Text('login.error_empty'.tr()),
                              backgroundColor: Colors.red,
                              behavior: SnackBarBehavior.floating,
                            ),
                          );
                          return;
                        }

                        setState(() {
                          _isLoading = true;
                        });

                        try {
                          final errorCode = await AuthService().loginUser(
                            email,
                            password,
                          );

                          if (!context.mounted) return;

                          if (errorCode == null) {
                            Navigator.pushReplacement(
                              context,
                              MaterialPageRoute(
                                builder: (context) => const HomeScreen(),
                              ),
                            );
                          } else {
                            String translationKey = 'errors.$errorCode';
                            String localizedMessage = translationKey.tr();

                            if (localizedMessage == translationKey) {
                              localizedMessage = 'errors.UNKNOWN_ERROR'.tr();
                            }

                            ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(
                                content: Text(localizedMessage),
                                backgroundColor: Colors.red,
                                behavior: SnackBarBehavior.floating,
                              ),
                            );
                          }
                        } finally {
                          if (context.mounted) {
                            setState(() {
                              _isLoading = false;
                            });
                          }
                        }
                      },
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.deepPurple,
                  disabledBackgroundColor: Colors.deepPurple.withValues(
                    alpha: 0.7,
                  ),
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  elevation: 0,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
                child: _isLoading
                    ? const SizedBox(
                        height: 20,
                        width: 20,
                        child: CircularProgressIndicator(
                          color: Colors.white,
                          strokeWidth: 2.5,
                        ),
                      )
                    : Text(
                        'login.submit_btn'.tr(),
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
              ),
              const SizedBox(height: 24),

              // --- Register Link ---
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text(
                    '${'login.no_account'.tr()} ',
                    style: TextStyle(color: subtitleColor), // <-- Đã sửa
                  ),
                  TextButton(
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => const RegisterScreen(),
                        ),
                      );
                    },
                    style: TextButton.styleFrom(
                      padding: EdgeInsets.zero,
                      minimumSize: const Size(50, 30),
                      tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                    ),
                    child: Text(
                      'login.register_here'.tr(),
                      style: const TextStyle(
                        color: Colors.deepPurple,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
