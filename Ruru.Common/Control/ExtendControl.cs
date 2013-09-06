namespace Ruru.Common.Control
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Control의 Invoke에 대한 확장 메소드 제공
    /// </summary>
    /// <example>int count = f.InvokeSync<int>(() => f.Controls.Count);</example>
    public static class ExtendControl
    {
        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TRet InvokeSync<TRet>(this Control c, Func<TRet> func)
        {
            if (c.InvokeRequired)
                return (TRet)c.Invoke(func);
            else
                return func();
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static TRet InvokeSync<T, TRet>(this Control c, Func<T, TRet> func, T argument)
        {
            if (c.InvokeRequired)
                return (TRet)c.Invoke(func, argument);
            else
                return func(argument);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="argument1"></param>
        /// <param name="argument2"></param>
        /// <returns></returns>
        public static TRet InvokeSync<T1, T2, TRet>(this Control c, Func<T1, T2, TRet> func, T1 argument1, T2 argument2)
        {
            if (c.InvokeRequired)
                return (TRet)c.Invoke(func, argument1, argument2);
            else
                return func(argument1, argument2);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="argument1"></param>
        /// <param name="argument2"></param>
        /// <param name="argument3"></param>
        /// <returns></returns>
        public static TRet InvokeSync<T1, T2, T3, TRet>(this Control c, Func<T1, T2, T3, TRet> func, T1 argument1, T2 argument2,
            T3 argument3)
        {
            if (c.InvokeRequired)
                return (TRet)c.Invoke(func, argument1, argument2, argument3);
            else
                return func(argument1, argument2, argument3);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="argument1"></param>
        /// <param name="argument2"></param>
        /// <param name="argument3"></param>
        /// <param name="argument4"></param>
        /// <returns></returns>
        public static TRet InvokeSync<T1, T2, T3, T4, TRet>(this Control c, Func<T1, T2, T3, T4, TRet> func, T1 argument1, T2 argument2,
            T3 argument3, T4 argument4)
        {
            if (c.InvokeRequired)
                return (TRet)c.Invoke(func, argument1, argument2, argument3, argument4);
            else
                return func(argument1, argument2, argument3, argument4);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="c"></param>
        /// <param name="func"></param>
        public static void InvokeSync(this Control c, Action func)
        {
            if (c.InvokeRequired)
                c.Invoke(func);
            else
                func();
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="argument"></param>
        public static void InvokeSync<T>(this Control c, Action<T> func, T argument)
        {
            if (c.InvokeRequired)
                c.Invoke(func, argument);
            else
                func(argument);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="argument1"></param>
        /// <param name="argument2"></param>
        public static void InvokeSync<T1, T2>(this Control c, Action<T1, T2> func, T1 argument1, T2 argument2)
        {
            if (c.InvokeRequired)
                c.Invoke(func, argument1, argument2);
            else
                func(argument1, argument2);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="argument1"></param>
        /// <param name="argument2"></param>
        /// <param name="argument3"></param>
        public static void InvokeSync<T1, T2, T3>(this Control c, Action<T1, T2, T3> func, T1 argument1, T2 argument2,
            T3 argument3)
        {
            if (c.InvokeRequired)
                c.Invoke(func, argument1, argument2, argument3);
            else
                func(argument1, argument2, argument3);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="argument1"></param>
        /// <param name="argument2"></param>
        /// <param name="argument3"></param>
        /// <param name="argument4"></param>
        public static void InvokeSync<T1, T2, T3, T4>(this Control c, Action<T1, T2, T3, T4> func, T1 argument1, T2 argument2,
            T3 argument3, T4 argument4)
        {
            if (c.InvokeRequired)
                c.Invoke(func, argument1, argument2, argument3, argument4);
            else
                func(argument1, argument2, argument3, argument4);
        }
    }
}
