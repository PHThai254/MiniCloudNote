import 'package:flutter_test/flutter_test.dart';
// Lưu ý: Chữ 'frontend' dưới đây là tên package trong file pubspec.yaml của bạn
import 'package:frontend/main.dart';

void main() {
  testWidgets('Kiểm tra khởi động MiniCloudNote', (WidgetTester tester) async {
    // Build ứng dụng của chúng ta
    await tester.pumpWidget(const MiniCloudNoteApp());

    // Kiểm tra xem dòng chữ khởi động có xuất hiện trên màn hình không
    expect(find.text('MiniCloudNote - Sẵn Sàng!'), findsOneWidget);
  });
}
