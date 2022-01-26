using Carwash.Repositories;
using Carwash.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Carwash.Controllers
{
    public class PaymentHistoryController : ControllerBase
    {
        private readonly TokenUtility _tokenUtility;
        private readonly PaymentHistoryRepository _paymentHistoryRepository;

        public PaymentHistoryController(TokenUtility tokenUtility, PaymentHistoryRepository paymentHistoryRepository)
        {
            _tokenUtility = tokenUtility;
            _paymentHistoryRepository = paymentHistoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentHistory()
        {
            var jwtTokenClaims = _tokenUtility.GetFromHttpContext(HttpContext);

            var result = await _paymentHistoryRepository.GetPaymentHistoryForUser(jwtTokenClaims.UserId);

            return Ok(result);
        }
    }
}
