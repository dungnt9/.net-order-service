# Order Service

## Mô tả

Order Service là một microservice quản lý đơn hàng, được xây dựng theo Clean Architecture với .NET 8.

## Kiến trúc

```
src/
├── API/              # Presentation layer (Controllers)
├── Application/      # Business logic (MediatR Handlers, Queries, Commands)
├── Domain/           # Entities, Value Objects
└── Infrastructure/   # Database, External Services, gRPC Client
```

## Ports

| Protocol  | Port | Mô tả              |
| --------- | ---- | ------------------ |
| HTTP/REST | 6002 | REST API endpoints |

## Giao tiếp với các Service khác

### 1. Product Service (gRPC)

Order Service sử dụng gRPC để lấy thông tin sản phẩm từ Product Service.

**Cấu hình trong `appsettings.json`:**

```json
{
  "Services": {
    "ProductService": {
      "BaseUrl": "http://localhost:6001",
      "GrpcUrl": "http://localhost:6003"
    }
  }
}
```

**Interface `IProductGrpcClient`:**

```csharp
public interface IProductGrpcClient
{
    Task<ProductDto?> GetProductAsync(int productId);
    Task<IEnumerable<ProductDto>> GetProductsAsync(IEnumerable<int> productIds);
}
```

**Sử dụng trong Handlers:**

- `GetOrderHandler`: Lấy thông tin product qua gRPC khi get order by ID
- `GetOrdersHandler`: Batch lấy thông tin products qua gRPC khi get all orders

### 2. RabbitMQ (Socket/Message Queue)

Order Service sử dụng RabbitMQ để gửi notifications khi có thay đổi đơn hàng.

## Proto file

File `Infrastructure/Protos/product.proto` được generate thành gRPC client:

```protobuf
service ProductGrpc {
  rpc GetProduct (GetProductRequest) returns (ProductResponse);
  rpc GetProducts (GetProductsRequest) returns (GetProductsResponse);
}
```

## Chạy ứng dụng

```bash
cd src/API
dotnet run --launch-profile Develop
```

Service sẽ listen trên:

- REST API: http://localhost:6002
- Swagger UI: http://localhost:6002/swagger

## Dependencies

- .NET 8
- Entity Framework Core 8
- MediatR
- Grpc.Net.Client
- Google.Protobuf
- Pomelo.EntityFrameworkCore.MySql
- RabbitMQ.Client

## Flow lấy thông tin Product

```
┌─────────────┐     gRPC (port 6003)     ┌─────────────────┐
│   Order     │ ──────────────────────►  │    Product      │
│   Service   │                          │    Service      │
│  (port 6002)│ ◄──────────────────────  │   (port 6001,   │
└─────────────┘     ProductResponse      │    6003)        │
                                         └─────────────────┘
```

Khi gọi API `GET /api/orders` hoặc `GET /api/orders/{id}`:

1. Order Service query database lấy thông tin order
2. Order Service gọi gRPC tới Product Service để lấy thông tin product mới nhất
3. Trả về response với thông tin product realtime (fallback về cached data nếu gRPC fail)
