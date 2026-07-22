using Puerts;

namespace Preactor {
    /// <summary>
    ///     Configure the JsEnv before any script evaluates — typically
    ///     <c>JsEnv.UsingAction&lt;T&gt;()</c> / <c>JsEnv.UsingFunc&lt;T&gt;()</c> registrations
    ///     for custom delegate types that JS callbacks are passed as. Registering from a
    ///     controller's own Start (even deferred a frame) races the first render's effects,
    ///     which run off JS timers and can hand a delegate to C# before the registration lands.
    ///     <para>
    ///         <see cref="PreactRenderer" /> invokes this on every component of its GameObject,
    ///         every <see cref="PreactRenderer.GlobalObjects" /> entry and every
    ///         <see cref="PreactGlobal.GlobalObjects" /> entry implementing it — right after the
    ///         JsEnv is created and before preloads or the entry file run, on every initialize
    ///         including live reloads.
    ///     </para>
    /// </summary>
    public interface IJsEnvConfigurator {
        void ConfigureJsEnv(JsEnv jsEnv);
    }
}
