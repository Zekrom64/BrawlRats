using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tesseract.Core.Graphics;
using Tesseract.Core.Graphics.Accelerated;
using Tesseract.Core.Input;
using Tesseract.GLFW;
using Tesseract.GLFW.Services;
using Tesseract.OpenGL;
using Tesseract.OpenGL.Graphics;
using Tesseract.Vulkan;
using Tesseract.Vulkan.Graphics;

namespace BrawlRats.Graphics {

	public static class Display {

		public static IWindowSystem WindowSystem { get; private set; }

		public static IWindow Window { get; private set; }

		public static IInputSystem InputSystem { get; private set; }

		public static void Init() {
			GLFW3.Init();
			WindowSystem = new GLFWServiceWindowSystem();
			InputSystem = new GLFWServiceInputSystem();
			InitWindow(GraphicsType.OpenGL);
		}

		private static void InitWindow(GraphicsType type) {
			WindowAttributeList attribs = new() {
				{ WindowAttributes.Resizable, false }
			};

			switch(type) {
				case GraphicsType.OpenGL:
					attribs.Add(GLWindowAttributes.OpenGLWindow, true);
					break;
				case GraphicsType.Vulkan:
					attribs.Add(VKWindowAttributes.VulkanWindow, true);
					break;
				default:
					throw new ArgumentException($"Unsupported graphics type \"{type}\"", nameof(type));
			}

			Window = WindowSystem.CreateWindow("Brawl Rats", 800, 600, attribs);

			Window.OnClosing += () => GameMain.Running = false;
		}

		public static void RunEvents() {
			InputSystem.RunEvents();
			Thread.Sleep(10);
		}

		public static void Deinit() {
			Window.Dispose();
			GLFW3.Terminate();
		}

	}

}
