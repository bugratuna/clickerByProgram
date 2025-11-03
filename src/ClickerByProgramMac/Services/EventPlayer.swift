import Foundation
import CoreGraphics

final class EventPlayer {
    private var playbackCancelled = false
    private var loopCancellationFlags: [CGMouseButton: Bool] = [:]
    private let stateQueue = DispatchQueue(label: "EventPlayerStateQueue")

    func play(actions: [MacroAction], speedMultiplier: Double) async throws {
        playbackCancelled = false
        for action in actions {
            try Task.checkCancellation()
            if playbackCancelled { break }

            let adjustedDelay = action.delay / max(speedMultiplier, 0.01)
            if adjustedDelay > 0 {
                try await Task.sleep(nanoseconds: UInt64(adjustedDelay * 1_000_000_000))
            }

            try Task.checkCancellation()
            if playbackCancelled { break }

            dispatch(action: action)
        }
    }

    func stop() {
        playbackCancelled = true
    }

    func performClickLoop(button: CGMouseButton, delay: TimeInterval) async {
        stateQueue.sync {
            loopCancellationFlags[button] = false
        }

        while !Task.isCancelled && !isLoopCancelled(button: button) {
            let downType: CGEventType
            let upType: CGEventType

            switch button {
            case .left:
                downType = .leftMouseDown
                upType = .leftMouseUp
            case .right:
                downType = .rightMouseDown
                upType = .rightMouseUp
            default:
                downType = .otherMouseDown
                upType = .otherMouseUp
            }

            if let position = CGEvent(source: nil)?.location {
                if let downEvent = CGEvent(mouseEventSource: nil,
                                            mouseType: downType,
                                            mouseCursorPosition: position,
                                            mouseButton: button) {
                    downEvent.post(tap: .cghidEventTap)
                }

                if let upEvent = CGEvent(mouseEventSource: nil,
                                          mouseType: upType,
                                          mouseCursorPosition: position,
                                          mouseButton: button) {
                    upEvent.post(tap: .cghidEventTap)
                }
            }

            try? await Task.sleep(nanoseconds: UInt64(max(delay, 0.01) * 1_000_000_000))
        }

        stateQueue.sync {
            loopCancellationFlags[button] = true
        }
    }

    func stopClickLoop(button: CGMouseButton) {
        stateQueue.sync {
            loopCancellationFlags[button] = true
        }
    }

    private func isLoopCancelled(button: CGMouseButton) -> Bool {
        stateQueue.sync {
            loopCancellationFlags[button] ?? true
        }
    }

    private func dispatch(action: MacroAction) {
        switch action.type {
        case .keyDown:
            guard let keyCode = action.keyCode else { return }
            CGEvent(keyboardEventSource: nil, virtualKey: keyCode, keyDown: true)?.post(tap: .cghidEventTap)
        case .keyUp:
            guard let keyCode = action.keyCode else { return }
            CGEvent(keyboardEventSource: nil, virtualKey: keyCode, keyDown: false)?.post(tap: .cghidEventTap)
        case .mouseDown:
            guard let button = action.mouseButton else { return }
            let type: CGEventType = button == .left ? .leftMouseDown : (button == .right ? .rightMouseDown : .otherMouseDown)
            CGEvent(mouseEventSource: nil,
                    mouseType: type,
                    mouseCursorPosition: action.location ?? CGEvent(source: nil)?.location ?? .zero,
                    mouseButton: button)?.post(tap: .cghidEventTap)
        case .mouseUp:
            guard let button = action.mouseButton else { return }
            let type: CGEventType = button == .left ? .leftMouseUp : (button == .right ? .rightMouseUp : .otherMouseUp)
            CGEvent(mouseEventSource: nil,
                    mouseType: type,
                    mouseCursorPosition: action.location ?? CGEvent(source: nil)?.location ?? .zero,
                    mouseButton: button)?.post(tap: .cghidEventTap)
        case .mouseMove:
            let position = action.location ?? CGEvent(source: nil)?.location ?? .zero
            CGWarpMouseCursorPosition(position)
        case .leftClickLoop:
            Task.detached { [weak self] in
                await self?.performClickLoop(button: .left, delay: action.delay)
            }
        case .rightClickLoop:
            Task.detached { [weak self] in
                await self?.performClickLoop(button: .right, delay: action.delay)
            }
        }
    }
}

extension EventPlayer: @unchecked Sendable {}
