using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Partner
        : BaseEntity
    {
        public string Name { get; set; }

        public int NumberIssuedPromoCodes  { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<PartnerPromoCodeLimit> PartnerLimits { get; set; }

        /// <summary>
        /// Установка лимита на выдачу промокодов для партнера
        /// </summary>
        /// <param name="limit">Лимит</param>
        /// <param name="endDate">Дата окончания лимита</param>
        public Result SetPromoCodeLimit(int limit, DateTime endDate)
        {
            if (limit <= 0)
                return Result.Failure("Лимит должен быть больше 0");

            //Если партнер заблокирован, то нужно выдать исключение
            if (!this.IsActive)
                return Result.Failure("Данный партнер не активен");

            //Установка лимита партнеру
            var activeLimit = this.PartnerLimits.FirstOrDefault(x =>
                !x.CancelDate.HasValue);

            if (activeLimit != null)
            {
                //Если партнеру выставляется лимит, то мы 
                //должны обнулить количество промокодов, которые партнер выдал, если лимит закончился, 
                //то количество не обнуляется
                this.NumberIssuedPromoCodes = 0;

                //При установке лимита нужно отключить предыдущий лимит
                activeLimit.CancelDate = DateTime.Now;
            }

            var newLimit = new PartnerPromoCodeLimit()
            {
                Limit = limit,
                Partner = this,
                PartnerId = this.Id,
                CreateDate = DateTime.Now,
                EndDate = endDate,
            };

            this.PartnerLimits.Add(newLimit);

            return Result.Success();
        }
    }
}