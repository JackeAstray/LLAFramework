using System.Collections.Generic;

namespace GameLogin.DatePicker
{
    public interface IInject
    {
        void Inject<T>(IList<T> data);
        void Inject<T>(IList<T>[] datas);
    }
}