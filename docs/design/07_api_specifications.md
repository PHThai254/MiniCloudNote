# Thiết kế API (API Specifications)

## 1. Quy ước chung (Conventions)
* **Authentication:** Sử dụng JWT (JSON Web Token). Các API yêu cầu quyền đăng nhập cần truyền Header: `Authorization: Bearer <your_access_token>`.
* **Content-Type:** `application/json` cho mọi Request và Response (ngoại trừ API upload file dùng `multipart/form-data`).
* **Format Response chuẩn:** ```json
  {
    "success": true,
    "message": "Thao tác thành công",
    "data": { ... } // Hoặc null nếu có lỗi
  }

## 2. Nhóm Auth (Xác thực)

| Method | Endpoint | Mô tả | Yêu cầu Auth |
| :--- | :--- | :--- | :---: |
| `POST` | `/auth/register` | Đăng ký tài khoản mới | ❌ |
| `POST` | `/auth/login` | Đăng nhập cấp Token | ❌ |
| `POST` | `/auth/refresh-token` | Cấp lại Access Token mới | ❌ (Gửi Refresh Token) |

**Ví dụ Payload Login:**
* **Request Body:** 
    ```json
  { 
    "email": "user@example.com", 
    "password": "Password123!" 
  }

* **Response Data:** 
    ```json
  {
    "accessToken": "eyJhb...",
    "refreshToken": "d8a1f..."
  }

## 3. Nhóm Users (Người dùng & Hồ sơ)
| Method | Endpoint | Mô tả | Yêu cầu Auth |
| :--- | :--- | :--- | :---: |
| `GET` | `/users/profile` | Lấy thông tin hồ sơ của user hiện tại | ✅ |
| `PUT`| `/users/profile` | Cập nhật hồ sơ (Tên, Bio, Theme) | ✅ |

**Ví dụ Payload Cập nhật Profile:**
* **Request Body:** 
    ```json
   {
    "fullName": "Thai PH",
    "bio": "IT Student",
    "preferredTheme": "Dark"
   }

## 4. Nhóm Notes (Ghi chú chính)
| Method | Endpoint | Mô tả | Yêu cầu Auth |
| :--- | :--- | :--- | :---: |
| `GET` | `/notes` | Lấy danh sách ghi chú (hỗ trợ phân trang) | ✅ |
| `GET`| `/notes/{id}` | Lấy chi tiết một ghi chú theo ID | ✅ |
| `POST` | `/notes` | Tạo ghi chú mới | ✅ |
| `PUT`| `/notes/{id}` | Cập nhật toàn bộ nội dung ghi chú | ✅ |
| `PATCH` | `/notes/{id}/pin` | Ghim/Bỏ ghim ghi chú | ✅ |
| `DELETE`| `/notes/{id}` | Xóa mềm (Đưa vào thùng rác) | ✅ |

**Ví dụ Payload Tạo Ghi chú (`POST /notes`):**
* **Request Body:** 
    ```json
    {
    "title": "Học Flutter cơ bản",
    "content": "Các widget cơ bản: Container, Row, Column...",
    "type": 0,
    "colorHex": "#FFFFFF"
    }

## 5. Nhóm Sync (Đồng bộ hóa Offline-to-Online)
| Method | Endpoint | Mô tả | Yêu cầu Auth |
| :--- | :--- | :--- | :---: |
| `POST` | `/notes/sync` | Đẩy các ghi chú sửa dưới Local lên Server | ✅ |

**Ví dụ Payload Sync:** 
* **Request Body:** (Truyền lên một mảng các ghi chú có cờ `is_synced = false` từ SQLite/Isar)
```json 
{
  "unsyncedNotes": [
    { 
      "id": "uuid-1", 
      "title": "Sửa offline 1", 
      "content": "Nội dung...", 
      "updatedAt": "2026-03-11T10:00:00Z" 
    },
    { 
      "id": "uuid-2", 
      "title": "Sửa offline 2", 
      "content": "Nội dung...", 
      "updatedAt": "2026-03-11T10:05:00Z" 
    }
  ]
}
