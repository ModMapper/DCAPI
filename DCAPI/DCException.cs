using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DCAPI {
    /// <summary>디시인사이드 서버에서 요청을 거부할시에 Throw되는 예외입니다.</summary>
    public class DCException : ApplicationException {
        /// <summary>새 빈 디시인사이드 예외를 생성합니다.</summary>
        public DCException() {}

        /// <summary>해당 메시지의 디시인사이드 예외를 생성합니다.</summary>
        /// <param name="messege">예외의 Throw사유입니다.</param>
        public DCException(string messege) : base(messege) {}

        /// <summary>해당 메시지의 디시인사이드 예외를 생성합니다.</summary>
        /// <param name="messege">예외의 Throw사유입니다.</param>
        /// <param name="innerException">내부 예외 원인입니다.</param>
        public DCException(string messege, Exception innerException) : base(messege, innerException) {}

        /// <summary>Json으로부터 성공 여부와 메시지를 가져옵니다.</summary>
        /// <param name="element">성공 여부를 가져올 <see cref="JsonElement"/>입니다.</param>
        /// <returns>해당 결과의 성공 여부와, 메시지입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (bool result, string cause) GetResult(JsonElement element) {
            if(!element.TryGetProperty("result", out var value)) return (true, null);
            if(value.ValueKind != JsonValueKind.False) return (true, null);
            return (false, element.GetString("cause"));
        }

        /// <summary>Json으로부터 성공 여부를 가져오고 실패시 <see cref="DCException"/>예외를 Throw합니다.</summary>
        /// <param name="element">성공 여부를 가져올 <see cref="JsonElement"/>입니다.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckResult(JsonElement element) {
            var (result, cause) = GetResult(element);
            if(!result)
                if(cause == null)
                    throw new DCException();
                else
                    throw new DCException(cause);
        }
    }
}
