﻿namespace Adnc.Infra.IRepository
{
    public interface IEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}