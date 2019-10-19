namespace Phoenix.Infrastructure.Wpf.Behaviours {
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public static class EventHandlerGenerator {
        public static Delegate CreateDelegate(Type eventHandlerType, MethodInfo methodToInvoke, object methodInvoker) {
            var eventHandlerInfo = eventHandlerType.GetMethod("Invoke");
            var returnType = eventHandlerInfo.ReturnParameter.ParameterType;
            if (returnType != typeof (void))
                throw new ApplicationException(
                    "Delegate has a return type. This only supprts event handlers that are void");

            var delegateParameters = eventHandlerInfo.GetParameters();
            var hookupParameters = new Type[delegateParameters.Length + 1];
            hookupParameters[0] = methodInvoker.GetType();
            for (var i = 0; i < delegateParameters.Length; i++)
                hookupParameters[i + 1] = delegateParameters[i].ParameterType;

            var handler = new DynamicMethod("", null, hookupParameters, typeof (EventHandlerGenerator));

            var eventIl = handler.GetILGenerator();

            var local = eventIl.DeclareLocal(typeof (object[]));
            eventIl.Emit(OpCodes.Ldc_I4, delegateParameters.Length + 1);
            eventIl.Emit(OpCodes.Newarr, typeof (object));
            eventIl.Emit(OpCodes.Stloc, local);

            for (var i = 1; i < delegateParameters.Length + 1; i++) {
                eventIl.Emit(OpCodes.Ldloc, local);
                eventIl.Emit(OpCodes.Ldc_I4, i);
                eventIl.Emit(OpCodes.Ldarg, i);
                eventIl.Emit(OpCodes.Stelem_Ref);
            }

            eventIl.Emit(OpCodes.Ldloc, local);

            eventIl.Emit(OpCodes.Ldarg_0);

            eventIl.EmitCall(OpCodes.Call, methodToInvoke, null);

            eventIl.Emit(OpCodes.Pop);
            eventIl.Emit(OpCodes.Ret);

            return handler.CreateDelegate(eventHandlerType, methodInvoker);
        }
    }
}