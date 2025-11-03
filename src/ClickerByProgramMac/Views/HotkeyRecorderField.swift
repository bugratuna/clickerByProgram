import SwiftUI
import AppKit

struct HotkeyRecorderField: NSViewRepresentable {
    @Binding var hotkey: Hotkey
    var placeholder: String

    func makeNSView(context: Context) -> HotkeyRecorderTextField {
        let textField = HotkeyRecorderTextField()
        textField.placeholderString = placeholder
        textField.isEditable = false
        textField.isBezeled = true
        textField.font = .systemFont(ofSize: NSFont.systemFontSize)
        textField.onHotkeyCaptured = { newHotkey in
            context.coordinator.binding.wrappedValue = newHotkey
            textField.stringValue = newHotkey.displayValue
        }
        textField.stringValue = hotkey.displayValue
        return textField
    }

    func updateNSView(_ nsView: HotkeyRecorderTextField, context: Context) {
        nsView.stringValue = hotkey.displayValue
    }

    func makeCoordinator() -> Coordinator {
        Coordinator(binding: $hotkey)
    }

    final class Coordinator {
        var binding: Binding<Hotkey>

        init(binding: Binding<Hotkey>) {
            self.binding = binding
        }
    }
}

final class HotkeyRecorderTextField: NSTextField {
    var onHotkeyCaptured: ((Hotkey) -> Void)?

    override func keyDown(with event: NSEvent) {
        let filteredModifiers = event.modifierFlags.intersection([.command, .option, .control, .shift])
        let newHotkey = Hotkey(keyCode: UInt32(event.keyCode), modifiers: filteredModifiers)
        onHotkeyCaptured?(newHotkey)
    }
}
