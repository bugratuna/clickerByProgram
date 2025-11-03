import Foundation
import Combine
import CoreGraphics
import QuartzCore

final class EventRecorder {
    private let eventsSubject = PassthroughSubject<[MacroAction], Never>()
    private let recordingStateSubject = CurrentValueSubject<Bool, Never>(false)

    var eventsPublisher: AnyPublisher<[MacroAction], Never> {
        eventsSubject.eraseToAnyPublisher()
    }

    var recordingStatePublisher: AnyPublisher<Bool, Never> {
        recordingStateSubject.removeDuplicates().eraseToAnyPublisher()
    }

    private var eventTap: CFMachPort?
    private var runLoopSource: CFRunLoopSource?
    private var recordedEvents: [MacroAction] = []
    private var lastTimestamp: CFTimeInterval?

    func startRecording() {
        guard !recordingStateSubject.value else { return }

        recordedEvents.removeAll()
        lastTimestamp = CACurrentMediaTime()

        let mask = eventMask(for: .keyDown) |
                   eventMask(for: .keyUp) |
                   eventMask(for: .leftMouseDown) |
                   eventMask(for: .leftMouseUp) |
                   eventMask(for: .rightMouseDown) |
                   eventMask(for: .rightMouseUp) |
                   eventMask(for: .otherMouseDown) |
                   eventMask(for: .otherMouseUp) |
                   eventMask(for: .mouseMoved) |
                   eventMask(for: .leftMouseDragged) |
                   eventMask(for: .rightMouseDragged) |
                   eventMask(for: .otherMouseDragged)

        let callback: CGEventTapCallBack = { _, type, event, refcon in
            guard let refcon = refcon else { return Unmanaged.passUnretained(event) }
            let recorder = Unmanaged<EventRecorder>.fromOpaque(refcon).takeUnretainedValue()
            recorder.handleEvent(type: type, event: event)
            return Unmanaged.passUnretained(event)
        }

        let refcon = Unmanaged.passUnretained(self).toOpaque()

        guard let eventTap = CGEventTapCreate(.cgSessionEventTap,
                                              .headInsertEventTap,
                                              .defaultTap,
                                              CGEventMask(mask),
                                              callback,
                                              refcon) else {
            return
        }

        self.eventTap = eventTap

        runLoopSource = CFMachPortCreateRunLoopSource(kCFAllocatorDefault, eventTap, 0)
        if let runLoopSource {
            CFRunLoopAddSource(CFRunLoopGetCurrent(), runLoopSource, .commonModes)
        }

        CGEventTapEnable(eventTap, true)
        recordingStateSubject.send(true)
    }

    func stopRecording() {
        guard recordingStateSubject.value else { return }

        if let eventTap {
            CGEventTapEnable(eventTap, false)
            CFMachPortInvalidate(eventTap)
            self.eventTap = nil
        }

        if let runLoopSource {
            CFRunLoopRemoveSource(CFRunLoopGetCurrent(), runLoopSource, .commonModes)
            self.runLoopSource = nil
        }

        recordingStateSubject.send(false)
        eventsSubject.send(recordedEvents)
    }

    private func handleEvent(type: CGEventType, event: CGEvent) {
        let now = CACurrentMediaTime()
        let delay: TimeInterval
        if let lastTimestamp {
            delay = now - lastTimestamp
        } else {
            delay = 0
        }
        lastTimestamp = now

        switch type {
        case .keyDown:
            let action = MacroAction(type: .keyDown,
                                     keyCode: event.getIntegerValueField(.keyboardEventKeycode).toCGKeyCode(),
                                     delay: delay)
            recordedEvents.append(action)
        case .keyUp:
            let action = MacroAction(type: .keyUp,
                                     keyCode: event.getIntegerValueField(.keyboardEventKeycode).toCGKeyCode(),
                                     delay: delay)
            recordedEvents.append(action)
        case .leftMouseDown:
            recordedEvents.append(MacroAction(type: .mouseDown,
                                              mouseButton: .left,
                                              location: event.location,
                                              delay: delay))
        case .leftMouseUp:
            recordedEvents.append(MacroAction(type: .mouseUp,
                                              mouseButton: .left,
                                              location: event.location,
                                              delay: delay))
        case .rightMouseDown:
            recordedEvents.append(MacroAction(type: .mouseDown,
                                              mouseButton: .right,
                                              location: event.location,
                                              delay: delay))
        case .rightMouseUp:
            recordedEvents.append(MacroAction(type: .mouseUp,
                                              mouseButton: .right,
                                              location: event.location,
                                              delay: delay))
        case .otherMouseDown:
            recordedEvents.append(MacroAction(type: .mouseDown,
                                              mouseButton: .center,
                                              location: event.location,
                                              delay: delay))
        case .otherMouseUp:
            recordedEvents.append(MacroAction(type: .mouseUp,
                                              mouseButton: .center,
                                              location: event.location,
                                              delay: delay))
        case .mouseMoved, .leftMouseDragged, .rightMouseDragged, .otherMouseDragged:
            recordedEvents.append(MacroAction(type: .mouseMove,
                                              location: event.location,
                                              delay: delay))
        default:
            break
        }
    }
}

private extension Int64 {
    func toCGKeyCode() -> CGKeyCode {
        CGKeyCode(self)
    }
}

private func eventMask(for type: CGEventType) -> CGEventMask {
    CGEventMask(1) << CGEventMask(type.rawValue)
}
