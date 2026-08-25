using Clinical.Api.Contracts;
using Clinical.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken ct);

        Task<Result<AuthResponse>> LoginAsync(
            LoginRequest request,
            CancellationToken ct);

        Task<Result<AuthResponse>> RefreshAsync(
            RefreshRequest request,
            CancellationToken ct);
    }
}
