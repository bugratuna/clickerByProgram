import AppKit
import Carbon

final class HotkeyManager {
    struct Token: Hashable {
        fileprivate let identifier: UInt32
    }

    private var nextId: UInt32 = 1
    private var hotkeyRefs: [UInt32: EventHotKeyRef?] = [:]
    private var handlers: [UInt32: () -> Void] = [:]
    private var eventHandler: EventHandlerRef?

    init() {
        installEventHandlerIfNeeded()
    }

    deinit {
        unregisterAll()
    }

    func register(key: Hotkey, handler: @escaping () -> Void) -> Token? {
        let id = nextId
        nextId += 1

        guard let ref = registerHotkey(id: id, key: key) else { return nil }
        hotkeyRefs[id] = ref
        handlers[id] = handler
        return Token(identifier: id)
    }

    func update(token: Token, to key: Hotkey) {
        let handler = handlers[token.identifier]
        if let ref = hotkeyRefs[token.identifier] {
            UnregisterEventHotKey(ref)
        }
        guard let ref = registerHotkey(id: token.identifier, key: key) else {
            hotkeyRefs[token.identifier] = nil
            if let handler {
                handlers[token.identifier] = handler
            }
            return
        }
        hotkeyRefs[token.identifier] = ref
        if let handler {
            handlers[token.identifier] = handler
        }
    }

    func unregister(token: Token) {
        if let ref = hotkeyRefs[token.identifier] {
            UnregisterEventHotKey(ref)
        }
        hotkeyRefs.removeValue(forKey: token.identifier)
        handlers.removeValue(forKey: token.identifier)
    }

    func unregisterAll() {
        for ref in hotkeyRefs.values {
            if let ref { UnregisterEventHotKey(ref) }
        }
        hotkeyRefs.removeAll()
        handlers.removeAll()
    }

    private func registerHotkey(id: UInt32, key: Hotkey) -> EventHotKeyRef? {
        let modifiers = modifiersForHotkey(key)
        var ref: EventHotKeyRef?
        var hotkeyID = EventHotKeyID(signature: OSType(bitPattern: UInt32(truncatingIfNeeded: "CLPR".fourCharCodeValue)),
                                     id: id)
        let status = RegisterEventHotKey(key.keyCode, modifiers, hotkeyID, GetEventDispatcherTarget(), 0, &ref)
        return status == noErr ? ref : nil
    }

    private func installEventHandlerIfNeeded() {
        guard eventHandler == nil else { return }
        let eventHandler: EventHandlerUPP = { _, event, userData in
            guard let event = event else { return noErr }
            let eventKind = GetEventKind(event)
            if eventKind == UInt32(kEventHotKeyPressed) {
                var hotkeyID = EventHotKeyID()
                GetEventParameter(event,
                                  EventParamName(kEventParamDirectObject),
                                  EventParamType(typeEventHotKeyID),
                                  nil,
                                  MemoryLayout<EventHotKeyID>.size,
                                  nil,
                                  &hotkeyID)

                let manager = Unmanaged<HotkeyManager>.fromOpaque(userData!).takeUnretainedValue()
                manager.handlers[hotkeyID.id]?()
            }
            return noErr
        }

        let userData = Unmanaged.passUnretained(self).toOpaque()
        var eventType = EventTypeSpec(eventClass: OSType(kEventClassKeyboard), eventKind: UInt32(kEventHotKeyPressed))
        InstallEventHandler(GetEventDispatcherTarget(), eventHandler, 1, &eventType, userData, &self.eventHandler)
    }

    private func modifiersForHotkey(_ hotkey: Hotkey) -> UInt32 {
        var carbonModifiers: UInt32 = 0
        if hotkey.modifiers.contains(.command) { carbonModifiers |= UInt32(cmdKey) }
        if hotkey.modifiers.contains(.option) { carbonModifiers |= UInt32(optionKey) }
        if hotkey.modifiers.contains(.control) { carbonModifiers |= UInt32(controlKey) }
        if hotkey.modifiers.contains(.shift) { carbonModifiers |= UInt32(shiftKey) }
        return carbonModifiers
    }
}

private extension String {
    var fourCharCodeValue: UInt32 {
        var result: UInt32 = 0
        for scalar in unicodeScalars {
            result = (result << 8) + UInt32(scalar.value)
        }
        return result
    }
}
