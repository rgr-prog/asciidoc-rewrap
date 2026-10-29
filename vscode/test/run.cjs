const { resolve } = require('path')
const { downloadAndUnzipVSCode, runTests } = require('@vscode/test-electron')

async function main() {
  try {
    // Electron-based terminals may pass this to Node. The test executable
    // must start VS Code, rather than interpret the workspace as a script.
    delete process.env.ELECTRON_RUN_AS_NODE
    const cachePath = resolve(__dirname, '../../.cache/vscode-test')
    const extensionDevelopmentPath = resolve(__dirname, '..')
    const extensionTestsPath = resolve(__dirname, 'tests.cjs')
    const workspace = resolve(__dirname, 'fixture')
    const extensionsDir = resolve(__dirname, 'fixture/extensions')
    const userDataDir = resolve(__dirname, '../../.cache/vscode-test/user-data')
    // --disable-extensions cannot be used because the fixture extension must
    // load. this means built-in extensions will also be loaded, but that should
    // be fine.
    const launchArgs = [workspace, '--extensions-dir', extensionsDir, '--user-data-dir', userDataDir, '--disable-gpu']

    // Use the given VS Code (the nix dev shell provides one on Linux), or download
    // it manually so we can choose the location
    const vscodeExecutablePath =
      process.env.VSCODE_TEST_EXECUTABLE || await downloadAndUnzipVSCode({ cachePath })

    // Run the integration tests
    await runTests({ extensionDevelopmentPath, extensionTestsPath, launchArgs, reuseMachineInstall: false, vscodeExecutablePath })
    console.error("Tests passed.")
  }
  catch (err) {
    console.error("Tests failed.")
    console.error(err)
    process.exit(1)
  }
}

main()
