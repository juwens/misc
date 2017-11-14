using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    static class Program
    {
        private enum MyEnum
        {
            A,
            B,
            C
        }

        static void Main(string[] args)
        {
            var res = ConditonalExp<MyEnum>().Compile()("B");
        }
        
        private static Expression<Func<string, T>> SwitchCaseWithResVariable<T>()
        {
            var res = Expression.Parameter(typeof(T));
            var strToParse = Expression.Parameter(typeof(string), "stringToParse");

            var cases = new List<SwitchCase>();
            foreach (var name in Enum.GetNames(typeof(MyEnum)))
            {
                var assign = Expression.Assign(res, Expression.Constant(Enum.Parse(typeof(T), name), typeof(T)));
                var switchCaseExp = Expression.SwitchCase(assign, Expression.Constant(name));
                cases.Add(switchCaseExp);
            }

            var exp = Expression.Switch(
                type: typeof(T),
                switchValue: strToParse,
                defaultBody: Expression.Constant(default(T)),
                comparison: null,
                cases: cases);

            var lambda = Expression.Lambda<Func<string, T>>(
                body: Expression.Block(new[] { res }, exp),
                parameters: strToParse);

            return lambda;
        }

        private static Expression<Func<string, T>> SwitchCaseWithoutVariable<T>()
        {
            var returnTarget = Expression.Label(typeof(T));
            var strToParse = Expression.Parameter(typeof(string), "stringToParse");

            var cases = new List<SwitchCase>();
            foreach (var name in Enum.GetNames(typeof(MyEnum)))
            {
                var returnExp = Expression.Return(returnTarget, Expression.Constant(Enum.Parse(typeof(T), name), typeof(T)), typeof(T));
                var switchCaseExp = Expression.SwitchCase(returnExp, Expression.Constant(name));
                cases.Add(switchCaseExp);
            }

            var switchExp = Expression.Switch(
                type: typeof(void),
                switchValue: strToParse,
                defaultBody: null,
                //defaultBody: Expression.Constant(default(T)),
                comparison: null,
                cases: cases);

            var returnLabel = Expression.Label(returnTarget, Expression.Constant(default(T)));

            var lambda = Expression.Lambda<Func<string, T>>(
                body: Expression.Block(switchExp, returnLabel),
                parameters: strToParse);

            return lambda;
        }

        private static Expression<Func<string, T>> ConditonalExp<T>()
        {
            var strToParse = Expression.Parameter(typeof(string), "stringToParse");

            Expression exp = Expression.Constant(default(T));
            foreach (var name in Enum.GetNames(typeof(MyEnum)))
            {
                exp = Expression.Condition(
                    test: Expression.Equal(Expression.Constant(name), strToParse),
                    ifTrue: Expression.Constant(Enum.Parse(typeof(T), name), typeof(T)),
                    ifFalse: exp);
            }

            var lambda = Expression.Lambda<Func<string, T>>(
                body: exp,
                parameters: strToParse);

            return lambda;
        }
    }
}
