// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.JSInterop.Implementation
{
    /// <summary>
    /// Implements functionality for <see cref="IJSObjectReference"/>.
    /// </summary>
    public class JSObjectReference : IJSObjectReference
    {
        private readonly JSRuntime _jsRuntime;

        internal bool Disposed { get; private set; }

        /// <summary>
        /// The unique identifier assigned to this instance.
        /// </summary>
        protected internal long Id { get; }

        /// <summary>
        /// Inititializes a new <see cref="JSObjectReference"/> instance.
        /// </summary>
        /// <param name="jsRuntime">The <see cref="JSRuntime"/> used for invoking JS interop calls.</param>
        /// <param name="id">The unique identifier.</param>
        protected internal JSObjectReference(JSRuntime jsRuntime, long id)
        {
            _jsRuntime = jsRuntime;

            Id = id;
        }

        /// <inheritdoc />
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            ThrowIfDisposed();

            return _jsRuntime.InvokeAsync<TValue>(Id, identifier, args);
        }

        /// <inheritdoc />
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            ThrowIfDisposed();

            return _jsRuntime.InvokeAsync<TValue>(Id, identifier, cancellationToken, args);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            
            Dispose(false);
            GC.SuppressFinalize(this);
        }
        
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (!Disposed)
            {
                Disposed = true;

                await _jsRuntime.InvokeVoidAsync("DotNet.jsCallDispatcher.disposeJSObjectReferenceById", Id);
            }
        }

        /// <inheritdoc />
        protected void ThrowIfDisposed()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
        
        ~JSObjectReference()
        {
            Dispose(false);
        }
    }
}
