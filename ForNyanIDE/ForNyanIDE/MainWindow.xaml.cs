// Created 2014 08 06 6:09 PM
// Changed 2014 08 06 6:29 PM

using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ForNyanIDE
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			string inputSource = new TextRange(InputSource.Document.ContentStart, InputSource.Document.ContentEnd).Text;
			Title = inputSource;
			NyanCompiler.Compiler.StartCompile(inputSource, "Nyan");
		}
	}
}