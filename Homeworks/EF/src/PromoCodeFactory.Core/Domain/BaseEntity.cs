using System;

namespace PromoCodeFactory.Core.Domain
{
    public class BaseEntity<T> where T : struct, IEquatable<T>
    {
        public T Id { get; set; }
    }
}