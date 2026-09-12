const __addToGlobal = function(name, obj) {
    const parts = name.split('.');
    let current = globalThis;

    for (let i = 0; i < parts.length - 1; i++) {
        current[parts[i]] = current[parts[i]] || {};
        current = current[parts[i]];
    }

    current[parts[parts.length - 1]] = obj;
};

const performance = {
    now: function () {
        return CS.UnityEngine.Time.realtimeSinceStartupAsDouble * 1000
    }
}

const requestAnimationFrame = function (callback) {
    return setTimeout(() => callback(CS.UnityEngine.Time.realtimeSinceStartupAsDouble * 1000))
}

const cancelAnimationFrame = function (id) {
    clearTimeout(id)
}

CS.UnityEngine.GameObject.prototype.GetComp = function (type) {
    return this.GetComponent(puer.$typeof(type))
}

CS.UnityEngine.GameObject.prototype.AddComp = function (type) {
    return this.AddComponent(puer.$typeof(type))
}

CS.UnityEngine.Component.prototype.GetComp = function (type) {
    return this.GetComponent(puer.$typeof(type))
}

CS.UnityEngine.Component.prototype.AddComp = function (type) {
    return this.AddComponent(puer.$typeof(type))
}

// puer.$promise registers a JS function as the C# task's continuation, which throws
// "JsEnv has been disposed" if the task outlives the view that awaited it. Route the
// continuation through interop.AwaitTask so it is dropped once the engine is gone. `interop`
// is added to the global scope after this preload runs, so it is looked up at call time.
const __taskToPromise = puer.$promise;
puer.$promise = function (task) {
    if (typeof interop === 'undefined') {
        return __taskToPromise(task);
    }

    return new Promise((resolve, reject) => {
        const settle = () => {
            const t = task;
            task = undefined;
            if (t.IsFaulted) {
                const e = t.Exception;
                reject(e ? (e.InnerException ? e.InnerException.Message : e.Message) : 'unknown exception!');
            } else if (t.IsCanceled) {
                reject('Task cancelled');
            } else {
                resolve(t.Result);
            }
        };

        try {
            interop.AwaitTask(task, settle);
        } catch {
            // Not a System.Threading.Tasks.Task (e.g. a boxed UniTask) — keep the stock behaviour.
            __taskToPromise(task).then(resolve, reject);
        }
    });
};
