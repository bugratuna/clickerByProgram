import Foundation
import Carbon

struct AppConfiguration: Codable {
    var startHotkey: Hotkey
    var stopHotkey: Hotkey
    var toggleHotkey: Hotkey
    var playbackSpeedMultiplier: Double
    var leftClickLoopDelay: TimeInterval
    var rightClickLoopDelay: TimeInterval

    static let `default` = AppConfiguration(startHotkey: .init(keyCode: UInt32(kVK_F7), modifiers: []),
                                            stopHotkey: .init(keyCode: UInt32(kVK_F8), modifiers: []),
                                            toggleHotkey: .init(keyCode: UInt32(kVK_F9), modifiers: []),
                                            playbackSpeedMultiplier: 1.0,
                                            leftClickLoopDelay: 0.2,
                                            rightClickLoopDelay: 0.2)
}
