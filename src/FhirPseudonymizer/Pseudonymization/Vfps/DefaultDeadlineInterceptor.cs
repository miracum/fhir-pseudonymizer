using Grpc.Core;
using Grpc.Core.Interceptors;

namespace FhirPseudonymizer.Pseudonymization.Vfps;

/// <summary>
///     Gives every unary call that doesn't already have a deadline a default one. Without it a
///     call waits until the caller cancels it - e.g. on a connection to a Vfps pod that silently
///     went away - and that cancellation is indistinguishable from the caller giving up for its
///     own reasons. With it, a hanging call fails as <see cref="StatusCode.DeadlineExceeded" />,
///     which is reported as transient.
///
///     The deadline covers the whole call, including the channel's own retries.
/// </summary>
internal sealed class DefaultDeadlineInterceptor(TimeSpan deadline) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation
    )
    {
        if (context.Options.Deadline is null)
        {
            context = new ClientInterceptorContext<TRequest, TResponse>(
                context.Method,
                context.Host,
                context.Options.WithDeadline(DateTime.UtcNow.Add(deadline))
            );
        }

        return continuation(request, context);
    }
}
