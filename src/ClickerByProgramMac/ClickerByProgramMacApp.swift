import SwiftUI

@main
struct ClickerByProgramMacApp: App {
    @NSApplicationDelegateAdaptor(AppDelegate.self) var appDelegate

    var body: some Scene {
        WindowGroup {
            ContentView()
                .environmentObject(appDelegate.state)
        }
        .commands {
            CommandGroup(after: .appTermination) {
                Button("Quit ClickerByProgram") {
                    NSApp.terminate(nil)
                }
                .keyboardShortcut("q", modifiers: [.command])
            }
        }
    }
}
