using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WolfGenerator.Core;
using WolfGenerator.Core.AST;
using WolfGenerator.Core.Parsing;

namespace ASTTree
{
	public partial class MainWindow
	{
		public static readonly DependencyProperty RuleClassProperty =
			DependencyProperty.Register( "RuleClass", typeof(RuleClassStatement), typeof(MainWindow),
			new PropertyMetadata(null, RuleClassPropertyChangedCallback));

		private static void RuleClassPropertyChangedCallback( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var w = d as Window;
			if (w == null) return;

			w.DataContext = e.NewValue;
		}

		public RuleClassStatement RuleClass
		{
			get { return (RuleClassStatement)this.GetValue( RuleClassProperty ); }
			set { this.SetValue( RuleClassProperty, value ); }
		}

		public MainWindow()
		{
			this.InitializeComponent();

			var fileName = @"E:\Himic\My Project\Unit.WolfGenerator\WolfGenerator.Core\CodeGenerator\Generator.rule";
			var text = new StreamReader( fileName ).ReadToEnd();

			var parser = new Parser( text );
			parser.Parse();

			this.RuleClass = parser.ruleClassStatement;
			this.TextBox.Text = text;
		}

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue is Statement)
			{
				var statement = (Statement)e.NewValue;

				var startPos = statement.StatementPosition.StartPos;
				var endPos = statement.StatementPosition.EndPos;
				try
				{
					this.TextBox.Select( startPos, endPos - startPos + 1 );
					//this.TextBox.Focus();
				}
				catch (Exception ex)
				{
					MessageBox.Show( ex.Message );
				}
			}
		}

		private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
		{
			this.SelectionStartTextBlock.Text = this.TextBox.SelectionStart.ToString();
			this.SelectionLengthTextBlock.Text = this.TextBox.SelectionLength.ToString();
		}
	}
}