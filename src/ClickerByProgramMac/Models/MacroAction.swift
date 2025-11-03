import Foundation
import CoreGraphics

enum MacroActionType: String, Codable {
    case keyDown
    case keyUp
    case mouseDown
    case mouseUp
    case mouseMove
    case leftClickLoop
    case rightClickLoop
}

struct MacroAction: Identifiable, Codable {
    let id: UUID
    var type: MacroActionType
    var keyCode: CGKeyCode?
    var mouseButton: CGMouseButton?
    var location: CGPoint?
    var delay: TimeInterval

    init(id: UUID = UUID(), type: MacroActionType, keyCode: CGKeyCode? = nil,
         mouseButton: CGMouseButton? = nil, location: CGPoint? = nil, delay: TimeInterval) {
        self.id = id
        self.type = type
        self.keyCode = keyCode
        self.mouseButton = mouseButton
        self.location = location
        self.delay = delay
    }
}
