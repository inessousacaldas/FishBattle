using System;
using System.Collections.Generic;

namespace UniRx
{
    public static class SubjectExtensions
    {
        public static Subject<T> CloseOnceNull<T>(this Subject<T> stream){
            if (stream != null) stream.Dispose();
            return null;
        }

        public static ISubject<T> Synchronize<T>(this ISubject<T> subject)
        {
            return new AnonymousSubject<T>((subject as IObserver<T>).Synchronize(), subject);
        }

        public static ISubject<T> Synchronize<T>(this ISubject<T> subject, object gate)
        {
            return new AnonymousSubject<T>((subject as IObserver<T>).Synchronize(gate), subject);
        }

        class AnonymousSubject<T, U> : ISubject<T, U>
        {
            readonly IObserver<T> observer;
            readonly IObservable<U> observable;

            public AnonymousSubject(IObserver<T> observer, IObservable<U> observable)
            {
                this.observer = observer;
                this.observable = observable;
            }

            public void OnCompleted()
            {
                observer.OnCompleted();
            }

            public void OnError(Exception error)
            {
                if (error == null) throw new ArgumentNullException("error");

                observer.OnError(error);
            }

            public void OnNext(T value)
            {
                observer.OnNext(value);
            }

            public IDisposable Subscribe(IObserver<U> observer)
            {
                if (observer == null) throw new ArgumentNullException("observer");

                return observable.Subscribe(observer);
            }
        }

        class AnonymousSubject<T> : AnonymousSubject<T, T>, ISubject<T>
        {
            public AnonymousSubject(IObserver<T> observer, IObservable<T> observable)
                : base(observer, observable)
            {
            }

            public IDisposable SubscribeAndFire(IObserver<T> observer)
            {
                throw new NotImplementedException();
            }

            public T LastValue { get; private set; }
        }

        public static IDisposable Combine(params IDisposable[] second)
        {
            return new ListDisposable(second);
        }

        public static IDisposable CombineRelease(this IDisposable first, IDisposable second)
        {
            if (first != null)
            {
                return new ListDisposable(new []{first,second});
            }
            else
            {
                return second;
            }
        }

        class ListDisposable:IDisposable
        {
            private IDisposable[] lst;

            public ListDisposable(IDisposable[] lst)
            {
                this.lst = lst;
            }

            public void Dispose()
            {
                if (lst == null) return;
                for (var i = 0; i < lst.Length; i++)
                {
                    if (lst[i] != null)
                    {
                        lst[i].Dispose();
                        lst[i] = null;
                    }
                }
                lst = null;
            }
        }
    }
}