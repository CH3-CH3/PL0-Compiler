   M S B A M L   `   `   `  ����E  ADragablz, Version=0.0.3.203, Culture=neutral, PublicKeyToken=null5(clr-namespace:Dragablz;assembly=DragablzDragablz  5       compiler   compiler.MainWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      H DMahApps.Metro, Version=1.6.5.1, Culture=neutral, PublicKeyToken=null;Controls,http://metro.mahapps.com/winfx/xaml/controls  5      7dragablz(clr-namespace:Dragablz;assembly=Dragablz  5      R NMahApps.Metro.IconPacks, Version=2.3.0.4, Culture=neutral, PublicKeyToken=null=	iconPacks-http://metro.mahapps.com/winfx/xaml/iconpacks  5        � Icon$  icon.ico��5      (  "MahApps.Metro.Controls.MetroWindow   ShowIconOnTitleBar .     	TitleCaps .     AccentColorBrush   	GlowBrush# �   5       � Title$ PL/0 Compiler��5       � 
ResizeMode$ CanResizeWithGrip=�5      $	��600��5      $	��800��5      $	��480��5      $	��640��5         IconTemplate 5      �� 5   
   .R�5         Dragablz.DragablzIcon  5         LeftWindowCommands 5      +  %MahApps.Metro.Controls.WindowCommands  5   
   .@�5      �� -   5         ��Name$ headerButton��5      .��5      �� 	 �� Orientation$	 
Horizontal=�6   .+�5      ,  &MahApps.Metro.IconPacks.PackIconModern  $��20��6/   $��20��5   +   E AControlzEx, Version=3.0.2.4, Culture=neutral, PublicKeyToken=null�  �ControlzEx.PackIconBase`1[[MahApps.Metro.IconPacks.PackIconModernKind, MahApps.Metro.IconPacks, Version=2.3.0.4, Culture=neutral, PublicKeyToken=null]]
   Kind$
 
WeatherSun=�5   +   5       5!      5"         RightWindowCommands 5%        5&   
   .@�5'      �� -   5        $ 
loadButton��5'      .��5(      �� $	 
Horizontal=�6   .+�5)      .  (MahApps.Metro.IconPacks.PackIconMaterial  $��16��61   $��16��5*   +   �  �ControlzEx.PackIconBase`1[[MahApps.Metro.IconPacks.PackIconMaterialKind, MahApps.Metro.IconPacks, Version=2.3.0.4, Culture=neutral, PublicKeyToken=null]]   Kind$ FileDownload=�5+   +   �� 5,      $��4 0 0 0q�6    $��Center=�5-      $
��Load��5.      5/      50      �� 51      -   5        $ 
saveButton��51      .��52      �� $	 
Horizontal=�6   .+�53        $��16��61   $��16��54   +   $ ContentSave=�55   +   �� 56      $��4 0 0 0q�6    $��Center=�57      $
��Save��58      59      5:      �� 5;      -   5        $ compileButton��5;      .��5<      �� $	 
Horizontal=�6   .+�5=        $��16��61   $��16��5>   +   $ Wrench=�5?   +   �� 5@      $��4 0 0 0q�6    $��Center=�5A      $��Compile��5B      5C      5D      �� 5E      -   5        $ 	runButton��5E      .��5F      �� $	 
Horizontal=�6   .+�5G        $��16��61   $��16��5H   +   $
 Play=�5I   +   �� 5J      $��4 0 0 0q�6    $��Center=�5K      $	��Run��5L      5M      5N      5O      .��5R      �  � RowDefinitions 5S   
   �� 5T      $��1* �6   �� 5U      $��20 �6   .+�5W   
   � $��0��6   $��0,0,-0.4,0.4q�6    � ColumnDefinitions 5X      �� 5Y      $��3* �6#   �� 5Z      $��3 �6#   �� 5[      $��2* �6#   .+�5^      �� $��1��6   $��3��6,   $��Stretch=�66   ���#F3F3F36k   � 5`      $��2��6   ���#F3F3F3.+�5b         Dragablz.TabablzControl  $��8q�6*   .@�5c      �� $��编译结果��6    �� 
IsSelected . .��5d      � .+�5e      &	   System.Windows.Controls.DataGrid	  -   5         	 Name$ 	dataGrid1��5e   N     AzureDataGrid#��[ 6'    	  AutoGenerateColumns .  $��0q�6}   /
  )MahApps.Metro.Controls.ScrollViewerHelper% 
  IsHorizontalScrollWheelEnabled .  	  Columns 5f   "   0  *System.Windows.Controls.DataGridTextColumn  5g   &   ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 6Q      Header$ 步骤��6      
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding 69   �� *Num+   ClipboardContentBinding 6\   Z� 6�     5h   &   $ 80 6R   $ 指令��6�    .  69   �� *INST+ 6]   Z� 6�     5i   &   $ 80 6R   $ 	层次差��6�    .  69   �� *Left+ 6]   Z� 6�     5j   &   $ 80 6S   $ 地址��6�    .  69   �� *Right+ 6^   Z� 6�   5l      5m      5n      5o      5p      � 5r      $��0��6    5s      �� 5t      $��9* �6$   �� 5u      $��3 �6$   �� 5v      $��5* �6$   .+�5x      ]	 YICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310' 	 !ICSharpCode.AvalonEdit.TextEditor  ?
avalonEdit.http://icsharpcode.net/sharpdevelop/avalonedit 	 5z      -   5        $��
textEditor��5{      $
��Auto=�5y      $��Consolas)�5|      $
��10pt(�5}      $��15q�6!   $��0��6-   �� 5~      $��1��6   $��3��6-   $��Stretch=�68   ���#F3F3F36m   � 5      $��2��6   .+�5�        $��8q�6.   .@�5�      �� -   5        $
��tab1��5�   0   $��编译信息��6"    . .��5�      �� -	   5        $��
textBlock1��5�   O    �� 
IsReadOnly . $��待命中...��6a   ��6&   Z� 6a   5�      �� 5�      -
   5        $
��tab2��5�   1   $��错误列表��6#   .��5�      � .+�5�   "   	  -   5        $ 	dataGrid2��5�   R   #��[ 6+    .  $��0q�6�    .  5�   &     5�   *   $ 50 6V   $ 行数��6�    .  6=   �� *Line+ 6a   Z� 6�     5�   *   $ 80 6[   $ 错误代码��6�    .  6=   �� *	ErrorCode+ 6f   Z� 6�     5�   *   $ 80 6X   $ 符号��6�    .  6=   �� *Detail+ 6c   Z� 6�     5�   *   $ 错误类型��6~    .  6=   �� *	ErrorInfo+ 6[   Z� 6�   5�   #   5�      5�      �� 5�      -   5        $
��tab3��5�   0   $��解释程序��6"   .��5�      �  5�   "   �� 5�   &   $��1* �67   �� 5�   &   $��2 �67   �� 5�   &   $��1* �67   .+�5�   "   �� -   5        $��inputBox��5�   &   $��0��5�   *    �� TextWrapping$
 Wrap=�5�   &   $
��Auto=�5�   &   *  $MahApps.Metro.Controls.TextBoxHelper   	Watermark$* $> 在此设置输入（默认为0）��5�   &   ��5�   &   Z� 5�   [   �� 5�   "   $��1��6/   $��2��6@   $��Stretch=�6J   ���#F3F3F36   �� 5�   "   -   5         ��Name$ textBox1��5�   %   $��2��5�   *    . $ > 在此获得输出��5�   %   $
��Auto=�5�   %   ��5�   %   Z� 6>   5�      5�      5�      5�      5�      5�      �� 5�   
   $��1��6   .@�5�      �� .��6   PL/0 Compiler6+   �� 5�        MetroStatusBarSeparator#��[ 6   �� 5�      .��6   Version 1.0.06+   5�      5�      5�      