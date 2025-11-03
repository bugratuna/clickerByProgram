import AppKit
import Combine

final class AppDelegate: NSObject, NSApplicationDelegate {
    let state = AppState()
    private var cancellables: Set<AnyCancellable> = []
    private let hotkeyManager = HotkeyManager()
    private var startToken: HotkeyManager.Token?
    private var stopToken: HotkeyManager.Token?
    private var toggleToken: HotkeyManager.Token?

    func applicationDidFinishLaunching(_ notification: Notification) {
        NSApp.setActivationPolicy(.regular)
        state.initializeServices()
        bindHotkeys()
    }

    func applicationWillTerminate(_ notification: Notification) {
        state.shutdown()
        hotkeyManager.unregisterAll()
    }

    private func bindHotkeys() {
        startToken = hotkeyManager.register(key: state.configuration.startHotkey) { [weak state] in
            state?.startPlayback()
        }

        stopToken = hotkeyManager.register(key: state.configuration.stopHotkey) { [weak state] in
            state?.stopAllAutomation()
        }

        toggleToken = hotkeyManager.register(key: state.configuration.toggleHotkey) { [weak state] in
            state?.togglePlayback()
        }

        state.$configuration
            .sink { [weak self] configuration in
                guard let self else { return }
                if let startToken {
                    hotkeyManager.update(token: startToken, to: configuration.startHotkey)
                }
                if let stopToken {
                    hotkeyManager.update(token: stopToken, to: configuration.stopHotkey)
                }
                if let toggleToken {
                    hotkeyManager.update(token: toggleToken, to: configuration.toggleHotkey)
                }
            }
            .store(in: &cancellables)
    }
}
