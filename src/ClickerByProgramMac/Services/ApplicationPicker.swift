import AppKit

final class ApplicationPicker {
    func fetchRunningApplications() -> [RunningApplication] {
        NSWorkspace.shared.runningApplications
            .filter { $0.activationPolicy == .regular && $0.bundleIdentifier != nil }
            .sorted { lhs, rhs in
                let leftName = lhs.localizedName ?? lhs.bundleIdentifier ?? ""
                let rightName = rhs.localizedName ?? rhs.bundleIdentifier ?? ""
                if leftName == rightName {
                    return lhs.processIdentifier < rhs.processIdentifier
                }
                return leftName < rightName
            }
            .compactMap { app in
                guard let bundleIdentifier = app.bundleIdentifier else { return nil }
                return RunningApplication(id: app.processIdentifier,
                                          name: app.localizedName ?? bundleIdentifier,
                                          bundleIdentifier: bundleIdentifier)
            }
    }
}
