using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordLinker_2.X.DiscordManage
{
    class DynamicEventHandler
    {
        public delegate Task EventHandled(EventInfo eventInfo, params object[] p);
        public static event EventHandled OnEventHandled;

        public static Delegate GetHandler(EventInfo ev)
        {
            string name = ev.Name;
            var parameters = ev.EventHandlerType.GetMethod("Invoke").GetParameters().
              Select((p, i) => Expression.Parameter(p.ParameterType, "p" + i)).ToArray();

            LambdaExpression lambdaExpression;

            switch (parameters.Length)
            {
                case 0:
                    lambdaExpression = Expression.Lambda(ev.EventHandlerType,
                        Expression.Call(typeof(DynamicEventHandler).GetMethod("PreEventHandler", new Type[] { typeof(EventInfo) }), 
                        Expression.Constant(ev)), parameters);
                    break;

                case 1:
                    lambdaExpression = Expression.Lambda(ev.EventHandlerType,
                        Expression.Call(typeof(DynamicEventHandler).GetMethod("PreEventHandler", new Type[] { typeof(EventInfo), typeof(object) }), 
                        Expression.Constant(ev), Expression.Convert(parameters[0], typeof(object))), parameters);
                    break;

                case 2:
                    lambdaExpression = Expression.Lambda(ev.EventHandlerType,
                        Expression.Call(typeof(DynamicEventHandler).GetMethod("PreEventHandler", new Type[] { typeof(EventInfo), typeof(object), typeof(object) }), 
                        Expression.Constant(ev), Expression.Convert(parameters[0], typeof(object)), Expression.Convert(parameters[1], typeof(object))), parameters);
                    break;

                case 3:
                    lambdaExpression = Expression.Lambda(ev.EventHandlerType,
                        Expression.Call(typeof(DynamicEventHandler).GetMethod("PreEventHandler", new Type[] { typeof(EventInfo), typeof(object), typeof(object), typeof(object) }), 
                        Expression.Constant(ev), Expression.Convert(parameters[1], typeof(object)), Expression.Convert(parameters[1], typeof(object)), Expression.Convert(parameters[2], typeof(object))), parameters);
                    break;

                default:
                    goto case 0;
            }

            return lambdaExpression.Compile();
        }

        public static void HandleAllEvents(object o)
        {
            foreach (var eventInfo in o.GetType().GetEvents())
            {
                var eventDelegate = GetHandler(eventInfo);

                eventInfo.AddEventHandler(o, eventDelegate);
            }
        }

        public static Task PreEventHandler(EventInfo eventInfo)
        {
            return EventHandler(eventInfo);
        }
        public static Task PreEventHandler(EventInfo eventInfo, object p0)
        {
            return EventHandler(eventInfo, p0);
        }
        public static Task PreEventHandler(EventInfo eventInfo, object p0, object p1)
        {
            return EventHandler(eventInfo, p0, p1);
        }
        public static Task PreEventHandler(EventInfo eventInfo, object p0, object p1, object p2)
        {
            return EventHandler(eventInfo, p0, p1, p2);
        }

        public static Task EventHandler(EventInfo eventInfo, params object[] p)
        {
            return OnEventHandled?.Invoke(eventInfo, p);
        }
    }
}