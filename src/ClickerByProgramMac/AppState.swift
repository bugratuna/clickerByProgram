import AppKit
import Combine
import CoreGraphics

final class AppState: ObservableObject {
    @Published var configuration: AppConfiguration = .default
    @Published var actions: [MacroAction] = []
    @Published var isRecording = false
    @Published var isPlaying = false
    @Published var isLeftClickLoopActive = false
    @Published var isRightClickLoopActive = false
    @Published var targetApplication: RunningApplication?
    @Published var availableApplications: [RunningApplication] = []

    private let recorder = EventRecorder()
    private let player = EventPlayer()
    private let applicationPicker = ApplicationPicker()
    private var cancellables: Set<AnyCancellable> = []
    private var playbackTask: Task<Void, Never>?
    private var leftClickLoopTask: Task<Void, Never>?
    private var rightClickLoopTask: Task<Void, Never>?

    func initializeServices() {
        recorder.eventsPublisher
            .receive(on: DispatchQueue.main)
            .sink { [weak self] events in
                self?.actions.append(contentsOf: events)
            }
            .store(in: &cancellables)

        recorder.recordingStatePublisher
            .receive(on: DispatchQueue.main)
            .assign(to: &$isRecording)

        refreshApplications()
    }

    func shutdown() {
        stopAllAutomation()
        recorder.stopRecording()
    }

    func refreshApplications() {
        availableApplications = applicationPicker.fetchRunningApplications()
        if targetApplication == nil {
            targetApplication = availableApplications.first
        }
    }

    func startRecording() {
        guard !isRecording else { return }
        actions.removeAll()
        recorder.startRecording()
    }

    func stopRecording() {
        recorder.stopRecording()
    }

    func startPlayback() {
        guard !isPlaying else { return }
        guard !actions.isEmpty else { return }

        stopLeftClickLoop()
        stopRightClickLoop()
        bringTargetAppToFront()
        isPlaying = true

        let actions = self.actions
        let speed = self.configuration.playbackSpeedMultiplier
        let player = self.player

        playbackTask = Task.detached { [weak self] in
            guard let self else { return }
            do {
                try await player.play(actions: actions,
                                       speedMultiplier: speed)
            } catch {
                await MainActor.run {
                    self.isPlaying = false
                    self.playbackTask = nil
                }
                return
            }

            await MainActor.run {
                self.isPlaying = false
                self.playbackTask = nil
            }
        }
    }

    func togglePlayback() {
        if isPlaying {
            stopPlayback()
        } else {
            startPlayback()
        }
    }

    func stopPlayback() {
        playbackTask?.cancel()
        playbackTask = nil
        player.stop()
        isPlaying = false
    }

    func startLeftClickLoop() {
        guard leftClickLoopTask == nil else { return }
        bringTargetAppToFront()
        isLeftClickLoopActive = true
        let player = self.player
        let delay = self.configuration.leftClickLoopDelay

        leftClickLoopTask = Task.detached { [weak self] in
            guard let self else { return }
            await player.performClickLoop(button: .left,
                                          delay: delay)
            await MainActor.run {
                self.isLeftClickLoopActive = false
                self.leftClickLoopTask = nil
            }
        }
    }

    func stopLeftClickLoop() {
        leftClickLoopTask?.cancel()
        leftClickLoopTask = nil
        player.stopClickLoop(button: .left)
        isLeftClickLoopActive = false
    }

    func startRightClickLoop() {
        guard rightClickLoopTask == nil else { return }
        bringTargetAppToFront()
        isRightClickLoopActive = true
        let player = self.player
        let delay = self.configuration.rightClickLoopDelay

        rightClickLoopTask = Task.detached { [weak self] in
            guard let self else { return }
            await player.performClickLoop(button: .right,
                                          delay: delay)
            await MainActor.run {
                self.isRightClickLoopActive = false
                self.rightClickLoopTask = nil
            }
        }
    }

    func stopRightClickLoop() {
        rightClickLoopTask?.cancel()
        rightClickLoopTask = nil
        player.stopClickLoop(button: .right)
        isRightClickLoopActive = false
    }

    func stopAllAutomation() {
        stopPlayback()
        stopLeftClickLoop()
        stopRightClickLoop()
    }

    func removeAction(at offsets: IndexSet) {
        actions.remove(atOffsets: offsets)
    }

    func moveAction(from source: IndexSet, to destination: Int) {
        actions.move(fromOffsets: source, toOffset: destination)
    }

    func addAction(_ action: MacroAction) {
        actions.append(action)
    }

    func bringTargetAppToFront() {
        guard let bundleIdentifier = targetApplication?.bundleIdentifier,
              let app = NSRunningApplication.runningApplications(withBundleIdentifier: bundleIdentifier).first else { return }
        app.activate(options: [.activateAllWindows, .activateIgnoringOtherApps])
    }
}
