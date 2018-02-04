using System;
using UniRx.InternalUtil;

namespace UniRx
{
    public sealed class Subject<T> : ISubject<T>, IDisposable, IOptimizedObservable<T>
    {
        object observerLock = new object();

        bool isStopped;
        bool isDisposed;
        Exception lastError;
        IObserver<T> outObserver = EmptyObserver<T>.Instance;

        public bool HasObservers
        {
            get
            {
                return !(outObserver is EmptyObserver<T>) && !isStopped && !isDisposed;
            }
        }

        public void OnCompleted()
        {
            IObserver<T> old;
            lock (observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                old = outObserver;
                outObserver = EmptyObserver<T>.Instance;
                isStopped = true;
                lastValue = default (T);
            }

            old.OnCompleted();
        }

        public void OnError(Exception error)
        {
            if (error == null) throw new ArgumentNullException("error");

            IObserver<T> old;
            lock (observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                old = outObserver;
                outObserver = EmptyObserver<T>.Instance;
                isStopped = true;
                lastError = error;
                lastValue = default (T);
            }

            old.OnError(error);
        }

        private T lastValue;
        public T LastValue {
            get { return lastValue;}
        }

        public void Hold(T value){
            lastValue = value;
        }

        public void OnNext(T value)
        {
            outObserver.OnNext(value);
            lastValue = value;
        }

        public IDisposable SubscribeAndFire(IObserver<T> observer)
        {
            var result = Subscribe(observer);
            if (!ReferenceEquals(Disposable.Empty, result))
            {
           //     if (lastValue != null)  //暂时不需要fire null
                    OnNext(lastValue);
            }
            return result;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");

            var ex = default(Exception);

            lock (observerLock)
            {
                ThrowIfDisposed();
                if (!isStopped)
                {
                    var listObserver = outObserver as ListObserver<T>;
                    if (listObserver != null)
                    {
                        outObserver = listObserver.Add(observer);
                    }
                    else
                    {
                        var current = outObserver;
                        if (current is EmptyObserver<T>)
                        {
                            outObserver = observer;
                        }
                        else
                        {
                            outObserver = new ListObserver<T>(new ImmutableList<IObserver<T>>(new[] { current, observer }));
                        }
                    }

                    return new Subscription(this, observer);
                }

                ex = lastError;
            }

            if (ex != null)
            {
                observer.OnError(ex);
            }
            else
            {
                observer.OnCompleted();
            }

            return Disposable.Empty;
        }

        public void Dispose()
        {
            lock (observerLock)
            {
                isDisposed = true;
                outObserver = DisposedObserver<T>.Instance;
            }
        }

        void ThrowIfDisposed()
        {
            if (isDisposed) throw new ObjectDisposedException("");
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }

        class Subscription : IDisposable
        {
            readonly object gate = new object();
            Subject<T> parent;
            IObserver<T> unsubscribeTarget;

            public Subscription(Subject<T> parent, IObserver<T> unsubscribeTarget)
            {
                this.parent = parent;
                this.unsubscribeTarget = unsubscribeTarget;
            }

            public void Dispose()
            {
                lock (gate)
                {
                    if (parent != null)
                    {
                        lock (parent.observerLock)
                        {
                            var listObserver = parent.outObserver as ListObserver<T>;
                            if (listObserver != null)
                            {
                                parent.outObserver = listObserver.Remove(unsubscribeTarget);
                            }
                            else
                            {
                                parent.outObserver = EmptyObserver<T>.Instance;
                            }

                            unsubscribeTarget = null;
                            parent = null;
                        }
                    }
                }
            }
        }
    }
}