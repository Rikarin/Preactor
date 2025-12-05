import glob from 'tiny-glob';
import * as esbuild from 'esbuild';
import chokidar from 'chokidar';
import { importTransformPlugin, watchOutputPlugin } from '@rikarin/preactor/scripts/esbuild/index.mjs';

const SOURCE_PATH = './dist/tsc/Views/*/index.js';

async function prepare() {
  try {
    let entryPoints = await glob(SOURCE_PATH);
    return await esbuild.context({
      entryPoints: entryPoints,
      bundle: true,
      plugins: [importTransformPlugin, watchOutputPlugin],
      inject: ['@rikarin/preactor/dist/index.js', './dist/tsc/globals.js'],
      alias: {
        preactor: '@rikarin/preactor',
        preact: '@rikarin/preactor/preact'
      },
      outdir: '../Assets/Views'
    });
  } catch { }
}

let main = await prepare();
await main?.watch();
console.log('Watching for changes...');


// Watch for new files added
const watcher = chokidar.watch('./dist', {
  persistent: true,
  ignoreInitial: true
});

let debounceTimeout;
const DEBOUNCE_INTERVAL = 100; // ms
watcher.on('add', async () => {
  clearTimeout(debounceTimeout);

  debounceTimeout = setTimeout(async () => {
    console.log('Detected new files, rebuilding...');
    await main?.dispose();
    main = await prepare();
    await main?.watch();
  }, DEBOUNCE_INTERVAL);
});

// Editor isn't supported, yet
// try {
// 	entryPoints = await glob("./dist/tsc/Editor/**/index.js");
// 	let editorCtx = await esbuild.context({
// 		entryPoints: entryPoints,
// 		bundle: true,
// 		globalName: "_contentScriptReturn",
// 		footer: { js: "_contentScriptReturn.default" },
// 		outdir: "../Assets/Editor/Views",
// 	});
// 	await editorCtx.watch();
// } catch (error) { }
