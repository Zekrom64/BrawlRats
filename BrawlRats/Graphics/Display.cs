using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tesseract.Core.Graphics;
using Tesseract.Core.Graphics.Accelerated;
using Tesseract.Core.Input;
using Tesseract.Core.Numerics;
using Tesseract.SDL;
using Tesseract.SDL.Services;
using Tesseract.OpenGL;

namespace BrawlRats.Graphics {

	/// <summary>
	/// The display class manages the state of the window the game is rendered to.
	/// </summary>
	public static class Display {

		/// <summary>
		/// The display's window system.
		/// </summary>
		public static IWindowSystem WindowSystem { get; private set; }

		/// <summary>
		/// The display's window.
		/// </summary>
		public static IWindow Window { get; private set; }

		/// <summary>
		/// The aspect ratio of the display.
		/// </summary>
		public static float AspectRatio {
			get {
				Vector2i size = Window.Size;
				return ((float)size.X) / size.Y;
			}
		}

		/// <summary>
		/// The display's input system.
		/// </summary>
		public static IInputSystem InputSystem { get; private set; }

		/// <summary>
		/// Initializes the display.
		/// </summary>
		public static void Init() {
			// Initialize GLFW
			SDL2.Init(SDLSubsystems.Video | SDLSubsystems.Joystick | SDLSubsystems.GameController);
			// Create the window and input systems
			WindowSystem = new SDLServiceWindowSystem();
			InputSystem = new SDLServiceInputSystem();
			// Initialize the window
			InitWindow();
		}

		// Initializes the window using the given graphics type
		private static void InitWindow() {
			WindowAttributeList attribs = new() {
				// Not resizable
				{ WindowAttributes.Resizable, false },
				// Initially not visible
				{ WindowAttributes.Visible, false },
				// Using OpenGL
				{ GLWindowAttributes.OpenGLWindow, true }
			};

			// Create the window
			Window = WindowSystem.CreateWindow("Brawl Rats", 800, 600, attribs);

			// On closing, game is shutting down immediately
			Window.OnClosing += () => GameMain.Running = false;
		}

		/// <summary>
		/// Runs event handling for the display.
		/// </summary>
		public static void RunEvents() {
			// Run events
			InputSystem.RunEvents();
			// Sleep for 10 milliseconds
			Thread.Sleep(10);
		}

		/// <summary>
		/// Deinitializes the display.
		/// </summary>
		public static void Deinit() {
			Window.Dispose();
			SDL2.Quit();
		}

	}

}
