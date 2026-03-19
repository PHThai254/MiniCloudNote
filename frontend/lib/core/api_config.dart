class ApiConfig {
  // Công tắc đổi thành true nếu chạy Máy ảo (Emulator)
  // Công tắc đổi thành false nếu cắm cáp Máy thật (Redmi)
  static const bool isEmulator = true;

  // Tự động phân luồng địa chỉ IP dựa vào công tắc trên
  static String get baseUrl {
    if (isEmulator) {
      // Cửa ngõ dành riêng cho Android Studio Emulator
      return 'http://10.0.2.2:5265/api';
    } else {
      // Cửa ngõ qua cáp USB (Phải chạy lệnh adb reverse)
      return 'http://127.0.0.1:5265/api';
    }
  }
}
