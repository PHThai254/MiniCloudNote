import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:frontend/features/auth/services/auth_service.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final TextEditingController _nameController = TextEditingController();
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();

  bool _isLoading = false;

  @override
  void dispose() {
    _nameController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    // Kỹ thuật bắt trạng thái Theme hiện tại
    final isDarkMode = Theme.of(context).brightness == Brightness.dark;

    // Các biến màu sắc linh hoạt
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
              // --- Nút Back & Nút Quả cầu ---
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  GestureDetector(
                    onTap: () {
                      Navigator.pop(context);
                    },
                    child: Container(
                      padding: const EdgeInsets.only(bottom: 16, right: 30),
                      color: Colors.transparent,
                      child: Icon(
                        Icons.arrow_back_ios_new,
                        color: textColor, // <-- Màu linh hoạt cho icon back
                        size: 28,
                      ),
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

              Text(
                'register.title'.tr(),
                style: TextStyle(
                  fontSize: 32,
                  fontWeight: FontWeight.bold,
                  color: textColor, // <-- Đã sửa
                ),
              ),
              const SizedBox(height: 8),
              Text(
                'register.subtitle'.tr(),
                style: TextStyle(
                  fontSize: 16,
                  color: subtitleColor,
                ), // <-- Đã sửa
              ),
              const SizedBox(height: 40),

              // --- Full Name Input ---
              Text(
                'register.fullname_label'.tr(),
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: textColor, // <-- Đã sửa
                ),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: _nameController,
                style: TextStyle(color: textColor), // <-- Ép màu chữ
                decoration: InputDecoration(
                  hintText: 'register.fullname_hint'.tr(),
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
              const SizedBox(height: 20),

              // --- Email Input ---
              Text(
                'register.email_label'.tr(),
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
                style: TextStyle(color: textColor), // <-- Ép màu chữ
                decoration: InputDecoration(
                  hintText: 'register.email_hint'.tr(),
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
              const SizedBox(height: 20),

              // --- Password Input ---
              Text(
                'register.password_label'.tr(),
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: textColor, // <-- Đã sửa
                ),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: _passwordController,
                obscureText: true,
                style: TextStyle(color: textColor), // <-- Ép màu chữ
                decoration: InputDecoration(
                  hintText: 'register.password_hint'.tr(),
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
              const SizedBox(height: 40),

              // --- Register Button (Logic giữ nguyên) ---
              ElevatedButton(
                onPressed: _isLoading
                    ? null
                    : () async {
                        final name = _nameController.text;
                        final email = _emailController.text;
                        final generatedUsername = email.split('@')[0];
                        final password = _passwordController.text;

                        if (name.isEmpty || email.isEmpty || password.isEmpty) {
                          ScaffoldMessenger.of(context).showSnackBar(
                            SnackBar(
                              content: Text('register.error_empty'.tr()),
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
                          final errorCode = await AuthService().registerUser(
                            generatedUsername,
                            name,
                            email,
                            password,
                          );

                          if (!context.mounted) return;

                          if (errorCode == null) {
                            ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(
                                content: Text('register.success_msg'.tr()),
                                backgroundColor: Colors.green,
                                behavior: SnackBarBehavior.floating,
                              ),
                            );
                            Navigator.pop(context);
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
                        'register.submit_btn'.tr(),
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
