import Foundation

struct RunningApplication: Identifiable, Equatable {
    let id: pid_t
    let name: String
    let bundleIdentifier: String
}
