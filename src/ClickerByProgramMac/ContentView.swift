import SwiftUI
import CoreGraphics

struct ContentView: View {
    @EnvironmentObject private var state: AppState

    var body: some View {
        VStack(alignment: .leading, spacing: 20) {
            header
            Divider()
            targetAppSection
            Divider()
            automationControls
            Divider()
            hotkeySection
            Divider()
            scriptSection
        }
        .padding(24)
        .frame(minWidth: 860, minHeight: 600)
    }

    private var header: some View {
        HStack {
            VStack(alignment: .leading) {
                Text("ClickerByProgram macOS")
                    .font(.largeTitle)
                    .bold()
                Text("Klavyeden ve fareden girdileri kaydedip tekrar oynatın.")
                    .foregroundStyle(.secondary)
            }
            Spacer()
            VStack(alignment: .trailing) {
                Button("Uygulamaları Yenile") {
                    state.refreshApplications()
                }
                .disabled(state.isRecording)
                Text(state.isRecording ? "Kaydediliyor" : state.isPlaying ? "Oynatılıyor" : "Hazır")
                    .font(.headline)
                    .foregroundStyle(state.isRecording ? .red : (state.isPlaying ? .green : .primary))
            }
        }
    }

    private var targetAppSection: some View {
        VStack(alignment: .leading, spacing: 12) {
            Text("Hedef Uygulama")
                .font(.title2)
            Picker("", selection: $state.targetApplication) {
                ForEach(state.availableApplications) { app in
                    Text(app.name).tag(Optional(app))
                }
            }
            .labelsHidden()
            .frame(maxWidth: 320)
        }
    }

    private var automationControls: some View {
        VStack(alignment: .leading, spacing: 16) {
            Text("Kayıt & Oynatma")
                .font(.title2)
            HStack(spacing: 12) {
                Button(state.isRecording ? "Kaydı Bitir" : "Kaydı Başlat") {
                    if state.isRecording {
                        state.stopRecording()
                    } else {
                        state.startRecording()
                    }
                }
                .keyboardShortcut(.space, modifiers: [])
                .disabled(state.isPlaying)

                Button(state.isPlaying ? "Oynatmayı Durdur" : "Oynat") {
                    if state.isPlaying {
                        state.stopPlayback()
                    } else {
                        state.startPlayback()
                    }
                }
                .disabled(state.actions.isEmpty || state.isRecording)

                Button(state.isLeftClickLoopActive ? "Sol Tıklama Döngüsünü Durdur" : "Sol Tıklama Döngüsü") {
                    if state.isLeftClickLoopActive {
                        state.stopLeftClickLoop()
                    } else {
                        state.startLeftClickLoop()
                    }
                }
                .disabled(state.isRecording)

                Button(state.isRightClickLoopActive ? "Sağ Tıklama Döngüsünü Durdur" : "Sağ Tıklama Döngüsü") {
                    if state.isRightClickLoopActive {
                        state.stopRightClickLoop()
                    } else {
                        state.startRightClickLoop()
                    }
                }
                .disabled(state.isRecording)
            }

            HStack(spacing: 16) {
                VStack(alignment: .leading) {
                    Text("Oynatma Hızı")
                    Slider(value: Binding(get: {
                        state.configuration.playbackSpeedMultiplier
                    }, set: { newValue in
                        state.configuration.playbackSpeedMultiplier = newValue
                    }), in: 0.25...4.0, step: 0.05)
                    Text(String(format: "%.2fx", state.configuration.playbackSpeedMultiplier))
                        .font(.caption)
                        .foregroundStyle(.secondary)
                }
                VStack(alignment: .leading) {
                    Text("Sol Tıklama Gecikmesi (sn)")
                    TextField("0.20", value: Binding(get: {
                        state.configuration.leftClickLoopDelay
                    }, set: { newValue in
                        state.configuration.leftClickLoopDelay = max(0.01, newValue)
                    }), format: .number.precision(.fractionLength(2)))
                    .textFieldStyle(.roundedBorder)
                    .frame(width: 100)
                }
                VStack(alignment: .leading) {
                    Text("Sağ Tıklama Gecikmesi (sn)")
                    TextField("0.20", value: Binding(get: {
                        state.configuration.rightClickLoopDelay
                    }, set: { newValue in
                        state.configuration.rightClickLoopDelay = max(0.01, newValue)
                    }), format: .number.precision(.fractionLength(2)))
                    .textFieldStyle(.roundedBorder)
                    .frame(width: 100)
                }
            }
        }
    }

    private var hotkeySection: some View {
        VStack(alignment: .leading, spacing: 12) {
            Text("Kısayol Tuşları")
                .font(.title2)
            HStack {
                VStack(alignment: .leading) {
                    Text("Başlat")
                    HotkeyRecorderField(hotkey: $state.configuration.startHotkey, placeholder: "Başlat Tuşu")
                        .frame(width: 180)
                }
                VStack(alignment: .leading) {
                    Text("Durdur")
                    HotkeyRecorderField(hotkey: $state.configuration.stopHotkey, placeholder: "Durdur Tuşu")
                        .frame(width: 180)
                }
                VStack(alignment: .leading) {
                    Text("Başlat/Durdur")
                    HotkeyRecorderField(hotkey: $state.configuration.toggleHotkey, placeholder: "Toggle Tuşu")
                        .frame(width: 180)
                }
            }
        }
    }

    private var scriptSection: some View {
        VStack(alignment: .leading, spacing: 12) {
            Text("Kayıt Edilen Adımlar")
                .font(.title2)
            if state.actions.isEmpty {
                Text("Henüz kayıt yok. 'Kaydı Başlat' ile kayıt almaya başlayın.")
                    .foregroundStyle(.secondary)
            } else {
                List {
                    ForEach(Array(state.actions.enumerated()), id: \.offset) { index, action in
                        HStack {
                            Text("\(index + 1).")
                                .frame(width: 32, alignment: .trailing)
                            Text(description(for: action))
                            Spacer()
                            Text(String(format: "%.2f sn", action.delay))
                                .foregroundStyle(.secondary)
                        }
                    }
                    .onDelete { indexSet in
                        state.removeAction(at: indexSet)
                    }
                    .onMove { indices, newOffset in
                        state.moveAction(from: indices, to: newOffset)
                    }
                }
                .frame(minHeight: 220)
                .toolbar { EditButton() }
            }
        }
    }

    private func description(for action: MacroAction) -> String {
        switch action.type {
        case .keyDown:
            return "Tuş basıldı: \(HotkeyFormatter.shared.name(for: UInt32(action.keyCode ?? 0)) ?? String(action.keyCode ?? 0))"
        case .keyUp:
            return "Tuş bırakıldı: \(HotkeyFormatter.shared.name(for: UInt32(action.keyCode ?? 0)) ?? String(action.keyCode ?? 0))"
        case .mouseDown:
            return "Fare basıldı: \(name(for: action.mouseButton))"
        case .mouseUp:
            return "Fare bırakıldı: \(name(for: action.mouseButton))"
        case .mouseMove:
            let point = action.location ?? .zero
            return String(format: "Fare hareketi: (%.0f, %.0f)", point.x, point.y)
        case .leftClickLoop:
            return "Sol tıklama döngüsü"
        case .rightClickLoop:
            return "Sağ tıklama döngüsü"
        }
    }

    private func name(for button: CGMouseButton?) -> String {
        switch button {
        case .left:
            return "Sol"
        case .right:
            return "Sağ"
        case .center:
            return "Orta"
        default:
            return "Bilinmiyor"
        }
    }
}
