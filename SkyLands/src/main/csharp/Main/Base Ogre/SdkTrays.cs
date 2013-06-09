/*
 -----------------------------------------------------------------------------
 This source file is part of OGRE
 (Object-oriented Graphics Rendering Engine)
 For the latest info, see http://www.ogre3d.org/
 
 Copyright (c) 2000-2009 Torus Knot Software Ltd
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
 -----------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using Mogre;
using GHA = Mogre.GuiHorizontalAlignment;
using TL = Game.BaseApp.TrayLocation;

namespace Game.BaseApp {
	public enum TrayLocation   // enumerator values for widget tray anchoring locations
	{
		TL_TOPLEFT,
		TL_TOP,
		TL_TOPRIGHT,
		TL_LEFT,
		TL_CENTER,
		TL_RIGHT,
		TL_BOTTOMLEFT,
		TL_BOTTOM,
		TL_BOTTOMRIGHT,
		TL_NONE
	}

	public enum ButtonState   // enumerator values for button states
	{
		BS_UP,
		BS_OVER,
		BS_DOWN
	}

	/*=============================================================================
	| Listener class for responding to tray events.
	=============================================================================*/
	public class SdkTrayListener {
		public virtual void buttonHit(Button button) {}
		public virtual void itemSelected(SelectMenu menu) {}
		public virtual void labelHit(Label label) {}
		public virtual void sliderMoved(Slider slider) {}
		public virtual void checkBoxToggled(CheckBox box) {}
		public virtual void okDialogClosed(string message) {}
		public virtual void yesNoDialogClosed(string question, bool yesHit) {}
    }

	/*=============================================================================
	| Abstract base class for all widgets.
	=============================================================================*/
	public class Widget {
		protected TrayLocation mTrayLoc;
        protected OverlayElement mElement;
        protected SdkTrayListener mListener;

		public Widget() {
			this.mTrayLoc = TrayLocation.TL_NONE;
			this.mElement = null;
			this.mListener = null;
		}

		public void cleanup() {
			if (this.mElement != null) nukeOverlayElement(this.mElement);
			this.mElement = null;
		}

		/*-----------------------------------------------------------------------------
		| Static utility method to recursively delete an overlay element plus
		| all of its children from the system.
		-----------------------------------------------------------------------------*/
		public static void nukeOverlayElement(OverlayElement element) {
            if(element == null || true) { return; }

            OverlayContainer container = (OverlayContainer)element;

            

            if(container != null) {
                List<OverlayElement> toDelete = new List<OverlayElement>();

                OverlayContainer.ChildIterator children = container.GetChildIterator();
                do {
                    toDelete.Add(children.Current);
                } while(children.MoveNext());

                foreach(OverlayElement t in toDelete)
                    nukeOverlayElement(t);

                OverlayContainer parent = element.Parent;
                if(parent != null) { parent.RemoveChild(element.Name); }
                OverlayManager.Singleton.DestroyOverlayElement(element);
            }
		}

		/*-----------------------------------------------------------------------------
		| Static utility method to check if the cursor is over an overlay element.
		-----------------------------------------------------------------------------*/
		public static bool isCursorOver(OverlayElement element, Vector2 cursorPos, float voidBorder = 0) {
			OverlayManager om = OverlayManager.Singleton;
            float l = element._getDerivedLeft() * om.ViewportWidth;
            float t = element._getDerivedTop() * om.ViewportHeight;
            float r = l + element.Width;
            float b = t + element.Height;

			return (cursorPos.x >= l + voidBorder && cursorPos.x <= r - voidBorder &&
				cursorPos.y >= t + voidBorder && cursorPos.y <= b - voidBorder);
		}

		/*-----------------------------------------------------------------------------
		| Static utility method used to get the cursor's offset from the center
		| of an overlay element in pixels.
		-----------------------------------------------------------------------------*/
		public static Vector2 cursorOffset(OverlayElement element, Vector2 cursorPos) {
			OverlayManager om = OverlayManager.Singleton;
			return new Vector2(cursorPos.x - (element._getDerivedLeft() * om.ViewportWidth + element.Width / 2),
				cursorPos.y - (element._getDerivedTop() * om.ViewportHeight + element.Height / 2));
		}

		/*-----------------------------------------------------------------------------
		| Static utility method used to get the width of a caption in a text area.
		-----------------------------------------------------------------------------*/
		public static float getCaptionWidth(string caption, TextAreaOverlayElement area) {
			ResourcePtr ft = FontManager.Singleton.GetByName(area.FontName);
            Font font = new Font(ft.Creator, ft.Name, ft.Handle, ft.Group, ft.IsManuallyLoaded);

			string current = caption;
			float lineWidth = 0;

			for (int i = 0; i < current.Length; i++) {
				// be sure to provide a line width in the text area
				if (current[i] == ' ')
				{
					if (area.SpaceWidth != 0) lineWidth += area.SpaceWidth;
					else lineWidth += font.GetGlyphAspectRatio(' ') * area.CharHeight;
				}
				else if (current[i] == '\n') break;
				// use glyph information to calculate line width
				else lineWidth += font.GetGlyphAspectRatio(current[i]) * area.CharHeight;
			}
			return (int)lineWidth;
		}

		/*-----------------------------------------------------------------------------
		| Static utility method to cut off a string to fit in a text area.
		-----------------------------------------------------------------------------*/
		public static void fitCaptionToArea(string caption, TextAreaOverlayElement area, float maxWidth) {
            ResourcePtr ft = FontManager.Singleton.GetByName(area.FontName);
            Font f = new Font(ft.Creator, ft.Name, ft.Handle, ft.Group, ft.IsManuallyLoaded);

			string s = caption;

			int nl = s.IndexOf('\n');
			if (nl != -1) s = s.Substring(0, nl);

			float width = 0;

			for (int i = 0; i < s.Length; i++) {
				if (s[i] == ' ' && area.SpaceWidth != 0) width += area.SpaceWidth;
				else width += f.GetGlyphAspectRatio(s[i]) * area.CharHeight;
				if (width > maxWidth)
				{
					s = s.Substring(0, i);
					break;
				}
			}

			area.Caption = s;
		}

		public OverlayElement getOverlayElement() { return this.mElement;           }
        public String getName()                   { return this.mElement != null ? this.mElement.Name : ""; }   // Not supposed to test != null
		public TrayLocation getTrayLocation()     { return mTrayLoc;         }
		public void hide()                        { this.mElement.Hide();           }
		public void show()                        { this.mElement.Show();           }
		public bool IsVisible()                   { return this.mElement.IsVisible; }

		// callbacks

		public virtual void _cursorPressed (Vector2 cursorPos) {}
		public virtual void _cursorReleased(Vector2 cursorPos) {}
		public virtual void _cursorMoved   (Vector2 cursorPos) {}
		public virtual void _focusLost()                       {}

		// internal methods used by SdkTrayManager. do not call directly.
		public void _assignToTray(TrayLocation trayLoc)       { this.mTrayLoc  = trayLoc;  }
		public void _assignListener(SdkTrayListener listener) { this.mListener = listener; }

	}

	/*=============================================================================
	| Basic button class.
	=============================================================================*/
	public class Button : Widget {
		// Do not instantiate any widgets directly. Use SdkTrayManager.

        protected ButtonState mState;
		protected BorderPanelOverlayElement mBP;
		protected TextAreaOverlayElement mTextArea;
		protected bool mFitToContents;

		public Button(String name, string caption, float width) {
			this.mElement = OverlayManager.Singleton.CreateOverlayElementFromTemplate("SdkTrays/Button", "BorderPanel", name);
			this.mBP = (BorderPanelOverlayElement)this.mElement;
			this.mTextArea = (TextAreaOverlayElement)this.mBP.GetChild(this.mBP.Name + "/ButtonCaption");
			this.mTextArea.Top = -(this.mTextArea.CharHeight / 2);

			if (width > 0) {
				this.mElement.Width = width;
				mFitToContents = false;
			}
			else mFitToContents = true;

			this.setCaption(caption);
            
			this.mState = ButtonState.BS_UP;

            this.mElement.Show();
            this.mBP.Show();
            this.mTextArea.Show();
		}


		public string getCaption() { return this.mTextArea.Caption; }
		public void   setCaption(string caption) {
			this.mTextArea.Caption = caption;
			if (this.mFitToContents) { this.mElement.Width = getCaptionWidth(caption, mTextArea) + this.mElement.Height - 12; }
		}

        public ButtonState getState() { return mState; }
		public override void _cursorPressed(Vector2 cursorPos) { if (isCursorOver(mElement, cursorPos, 4)) setState(ButtonState.BS_DOWN); }

		public override void _cursorReleased(Vector2 cursorPos) {
			if (mState == ButtonState.BS_DOWN) {
				setState(ButtonState.BS_OVER);
				if (mListener != null) mListener.buttonHit(this);
			}
		}

		public override void _cursorMoved(Vector2 cursorPos) {
			if (isCursorOver(this.mElement, cursorPos, 4)) { if (mState == ButtonState.BS_UP) setState(ButtonState.BS_OVER); }
			else                                           { if (mState != ButtonState.BS_UP) setState(ButtonState.BS_UP); }
		}

        // reset button if cursor was lost
		public override void _focusLost() { setState(ButtonState.BS_UP); }

		public void setState(ButtonState bs) {
			if (bs == ButtonState.BS_OVER) {
				this.mBP.BorderMaterialName = "SdkTrays/Button/Over";
				this.mBP.MaterialName       = "SdkTrays/Button/Over";
			}
			else if (bs == ButtonState.BS_UP) {
                this.mBP.BorderMaterialName = "SdkTrays/Button/Up";
				this.mBP.MaterialName       = "SdkTrays/Button/Up";
			}
			else {
                this.mBP.BorderMaterialName = "SdkTrays/Button/Down";
				this.mBP.MaterialName       = "SdkTrays/Button/Down";
			}

			this.mState = bs;
		}
	}

	/*=============================================================================
	| Scrollable text box widget.
	=============================================================================*/
	public class TextBox : Widget {
		// Do not instantiate any widgets directly. Use SdkTrayManager.

        protected TextAreaOverlayElement mTextArea;
		protected BorderPanelOverlayElement mCaptionBar;
		protected TextAreaOverlayElement mCaptionTextArea;
		protected BorderPanelOverlayElement mScrollTrack;
		protected PanelOverlayElement mScrollHandle;
		protected string mText;
		protected List<String> mLines;
		protected float mPadding;
		protected bool mDragging;
		protected float mScrollPercentage;
		protected float mDragOffset;
		protected int mStartingLine;


		public TextBox(String name, string caption, float width, float height)
		{
			this.mElement = OverlayManager.Singleton.CreateOverlayElementFromTemplate("SdkTrays/TextBox", "BorderPanel", name);
			this.mElement.Width = width;
			this.mElement.Height = height;
			OverlayContainer container = (OverlayContainer)this.mElement;
			this.mTextArea = (TextAreaOverlayElement)container.GetChild(this.getName() + "/TextBoxText");
			this.mCaptionBar = (BorderPanelOverlayElement)container.GetChild(this.getName() + "/TextBoxCaptionBar");
			this.mCaptionBar.Width = width - 4;
			this.mCaptionTextArea = (TextAreaOverlayElement)this.mCaptionBar.GetChild(this.mCaptionBar.Name + "/TextBoxCaption");
			this.setCaption(caption);
			this.mScrollTrack = (BorderPanelOverlayElement)container.GetChild(this.getName() + "/TextBoxScrollTrack");
			this.mScrollHandle = (PanelOverlayElement)this.mScrollTrack.GetChild(this.mScrollTrack.Name + "/TextBoxScrollHandle");
			this.mScrollHandle.Hide();
			this.mDragging = false;
			this.mScrollPercentage = 0;
			this.mStartingLine = 0;
			this.mPadding = 15;
			this.mText = "";
            this.mLines = new List<string>();
			this.refitContents();
		}

		public void setPadding(float padding) {
			this.mPadding = padding;
			this.refitContents();
		}

		public float  getPadding() { return mPadding; }
		public string getCaption() { return this.mCaptionTextArea.Caption; }
		public void   setCaption(string caption) { this.mCaptionTextArea.Caption = caption; }
		public string getText() { return mText; }

		/*-----------------------------------------------------------------------------
		| Sets text box content. Most of this method is for wordwrap.
		-----------------------------------------------------------------------------*/
		public void setText(string text) {
			this.mText = text;
			this.mLines.Clear();

            ResourcePtr ft = FontManager.Singleton.GetByName(this.mTextArea.FontName);
            Font font = new Font(ft.Creator, ft.Name, ft.Handle, ft.Group, ft.IsManuallyLoaded);
            
			String current = text;
			bool firstWord = true;
			int lastSpace = 0;
			int lineBegin = 0;
			float lineWidth = 0;
			float rightBoundary = this.mElement.Width - 2 * mPadding + this.mScrollTrack.Left + 10;

			for (int i = 0; i < current.Length; i++) {
				if (current[i] == ' ') {
					if (this.mTextArea.SpaceWidth != 0) lineWidth += this.mTextArea.SpaceWidth;
					else lineWidth += font.GetGlyphAspectRatio(' ') * this.mTextArea.CharHeight;
					firstWord = false;
					lastSpace = i;
				}
				else if (current[i] == '\n') {
					firstWord = true;
					lineWidth = 0;
					mLines.Add(current.Substring(lineBegin, i - lineBegin));
					lineBegin = i + 1;
				}
				else {
					// use glyph information to calculate line width
					lineWidth += font.GetGlyphAspectRatio(current[i]) * this.mTextArea.CharHeight;
					if (lineWidth > rightBoundary) {
						if (firstWord) {
							current.Insert(i, "\n");
							i = i - 1;
						}
						else {
                            char[] str = current.ToCharArray();
                            str[lastSpace] = '\n';
                            current = new String(str);
							i = lastSpace - 1;
						}
					}
				}
			}

			mLines.Add(current.Substring(lineBegin));

			int maxLines = getHeightInLines();

			if (mLines.Count > maxLines)     // if too much text, filter based on scroll percentage
			{
				this.mScrollHandle.Show();
				this.filterLines();
			}
			else       // otherwise just show all the text
			{
				this.mTextArea.Caption = current;
				this.mScrollHandle.Hide();
				this.mScrollPercentage = 0;
				this.mScrollHandle.Top = 0;
			}
		}

		/*-----------------------------------------------------------------------------
		| Sets text box content horizontal alignment.
		-----------------------------------------------------------------------------*/
		public void setTextAlignment(TextAreaOverlayElement.Alignment ta)
		{
			if (ta == TextAreaOverlayElement.Alignment.Left) this.mTextArea.HorizontalAlignment = GuiHorizontalAlignment.GHA_LEFT;
			else if (ta == TextAreaOverlayElement.Alignment.Center) this.mTextArea.HorizontalAlignment  = GuiHorizontalAlignment.GHA_CENTER;
			else this.mTextArea.HorizontalAlignment = GuiHorizontalAlignment.GHA_RIGHT;
			refitContents();
		}

		public void clearText() { setText(""); }

		public void appendText(string text) { setText(getText() + text); }

		/*-----------------------------------------------------------------------------
		| Makes adjustments based on new padding, size, or alignment info.
		-----------------------------------------------------------------------------*/
		public void refitContents()
		{
			this.mScrollTrack.Height = this.mElement.Height - this.mCaptionBar.Height - 20;
			this.mScrollTrack.Top = this.mCaptionBar.Height + 10;

			this.mTextArea.Top = this.mCaptionBar.Height + this.mPadding - 5;
			if (this.mTextArea.HorizontalAlignment == GuiHorizontalAlignment.GHA_RIGHT) this.mTextArea.Left = -this.mPadding + this.mScrollTrack.Left;
			else if (this.mTextArea.HorizontalAlignment == GuiHorizontalAlignment.GHA_LEFT) this.mTextArea.Left = this.mPadding;
			else this.mTextArea.Left = this.mScrollTrack.Left/ 2;

			this.setText(getText());
		}

		/*-----------------------------------------------------------------------------
		| Sets how far scrolled down the text is as a percentage.
		-----------------------------------------------------------------------------*/
		public void setScrollPercentage(float percentage) {
			this.mScrollPercentage = MathHelper.clamp(percentage, 0, 1);
			this.mScrollHandle.Top = (int)(percentage * (this.mScrollTrack.Height - this.mScrollHandle.Height));
			this.filterLines();
		}

		/*-----------------------------------------------------------------------------
		| Gets how far scrolled down the text is as a percentage.
		-----------------------------------------------------------------------------*/
		public float getScrollPercentage() { return this.mScrollPercentage; }

		/*-----------------------------------------------------------------------------
		| Gets how many lines of text can fit in this window.
		-----------------------------------------------------------------------------*/
		public int getHeightInLines() { return (int) ((this.mElement.Height - 2 * mPadding - this.mCaptionBar.Height + 5) / this.mTextArea.CharHeight); }

		public override void _cursorPressed(Vector2 cursorPos) {
			if (!this.mScrollHandle.IsVisible) return;   // don't care about clicks if text not scrollable

			Vector2 co = Widget.cursorOffset(mScrollHandle, cursorPos);

			if (co.SquaredLength <= 81) {
				this.mDragging = true;
				this.mDragOffset = co.y;
			}
			else if (Widget.isCursorOver(mScrollTrack, cursorPos)) {
				float newTop = this.mScrollHandle.Top+ co.y;
				float lowerBoundary = this.mScrollTrack.Height - this.mScrollHandle.Height;
				this.mScrollHandle.Top = MathHelper.clamp((int)newTop, 0, (int)lowerBoundary);

				// update text area contents based on new scroll percentage
				this.mScrollPercentage = MathHelper.clamp(newTop / lowerBoundary, 0, 1);
				this.filterLines();
			}
		}

		public override void _cursorReleased(Vector2 cursorPos) { this.mDragging = false; }

		public override void _cursorMoved(Vector2 cursorPos) {
			if (this.mDragging) {
				Vector2 co = Widget.cursorOffset(mScrollHandle, cursorPos);
				float newTop = this.mScrollHandle.Top + co.y - this.mDragOffset;
				float lowerBoundary = this.mScrollTrack.Height - this.mScrollHandle.Height;
				this.mScrollHandle.Top = MathHelper.clamp((int)newTop, 0, (int)lowerBoundary);

				// update text area contents based on new scroll percentage
				this.mScrollPercentage = MathHelper.clamp(newTop / lowerBoundary, 0, 1);
				this.filterLines();
			}
		}

        // stop dragging if cursor was lost
		public override void _focusLost() { mDragging = false; }

		/*-----------------------------------------------------------------------------
		| Decides which lines to show.
		-----------------------------------------------------------------------------*/
		protected void filterLines() {
			String shown = "";
			int maxLines = this.getHeightInLines();
			int newStart = (int) (this.mScrollPercentage * (this.mLines.Count - maxLines) + 0.5);

			mStartingLine = newStart;

			for (int i = 0; i < maxLines; i++) {
				shown += mLines[mStartingLine + i] + "\n";
			}

			this.mTextArea.Caption = shown;    // show just the filtered lines
		}
	}

	/*=============================================================================
	| Basic selection menu widget.
	=============================================================================*/
	public class SelectMenu : Widget
	{
		protected BorderPanelOverlayElement mSmallBox;
		protected BorderPanelOverlayElement mExpandedBox;
		protected TextAreaOverlayElement mTextArea;
		protected TextAreaOverlayElement mSmallTextArea;
		protected BorderPanelOverlayElement mScrollTrack;
		protected PanelOverlayElement mScrollHandle;
		protected List<BorderPanelOverlayElement> mItemElements;
		protected uint mMaxItemsShown;
		protected uint mItemsShown;
		protected bool mCursorOver;
		protected bool mExpanded;
		protected bool mFitToContents;
		protected bool mDragging;
		protected List<string> mItems;
		protected int mSelectionIndex;
		protected int mHighlightIndex;
		protected int mDisplayIndex;
		protected float mDragOffset;
        
        // Do not instantiate any widgets directly. Use SdkTrayManager.	
        public SelectMenu(string name, string caption, float width, float boxWidth, uint maxItemsShown)
		{
			this.mHighlightIndex = 0;
			this.mDisplayIndex = 0;
			this.mDragOffset = 0.0f;
            this.mSelectionIndex = -1;
			this.mFitToContents = false;
			this.mCursorOver = false;
			this.mExpanded = false;
			this.mDragging = false;
			this.mMaxItemsShown = maxItemsShown;
			this.mItemsShown = 0;
			this.mElement = OverlayManager.Singleton.CreateOverlayElementFromTemplate("SdkTrays/SelectMenu", "BorderPanel", name);
			this.mTextArea = (TextAreaOverlayElement) ((OverlayContainer) this.mElement).GetChild(name + "/MenuCaption");
			this.mSmallBox = (BorderPanelOverlayElement) ((OverlayContainer)this.mElement).GetChild(name + "/MenuSmallBox");
			this.mSmallBox.Width = width - 10;
			this.mSmallTextArea = (TextAreaOverlayElement) this.mSmallBox.GetChild(name + "/MenuSmallBox/MenuSmallText");
			this.mElement.Width = width;
            this.mItemElements = new List<BorderPanelOverlayElement>();
            this.mItems = new List<string>();
            
			if (boxWidth > 0)  // long style
			{
				if (width <= 0) { this.mFitToContents = true; }
				this.mSmallBox.Width = boxWidth;
				this.mSmallBox.Top = 2;
				this.mSmallBox.Left = width - boxWidth - 5;
				this.mElement.Height = this.mSmallBox.Height + 4;
				this.mTextArea.HorizontalAlignment = GuiHorizontalAlignment.GHA_LEFT;
				this.mTextArea.SetAlignment(TextAreaOverlayElement.Alignment.Left);
				this.mTextArea.Left = 12;
				this.mTextArea.Top = 10;
			}
						
			this.mExpandedBox = (BorderPanelOverlayElement) ((OverlayContainer)this.mElement).GetChild(name + "/MenuExpandedBox");
			this.mExpandedBox.Width = this.mSmallBox.Width + 10;
			this.mExpandedBox.Hide();
			this.mScrollTrack = (BorderPanelOverlayElement)this.mExpandedBox.GetChild(this.mExpandedBox.Name + "/MenuScrollTrack");
			this.mScrollHandle = (PanelOverlayElement)this.mScrollTrack.GetChild(this.mScrollTrack.Name + "/MenuScrollHandle");

			this.setCaption(caption);
		}

		public bool isExpanded() { return mExpanded; }

		public string getCaption() { return mTextArea.Caption; }

		public void setCaption(string caption)
		{
			this.mTextArea.Caption = caption;
			if (this.mFitToContents)
			{
				this.mElement.Width = getCaptionWidth(caption, this.mTextArea) + this.mSmallBox.Width + 23;
				this.mSmallBox.Left = this.mElement.Width - this.mSmallBox.Width - 5;
			}
		}

		public List<string> getItems() { return this.mItems; }

		public uint getNumItems() { return (uint) this.mItems.Count; }

		public void setItems(List<string> items)
		{
			this.mItems = items;
			this.mSelectionIndex = -1;

			foreach (BorderPanelOverlayElement t in this.mItemElements)
			    nukeOverlayElement(t);

		    this.mItemElements.Clear();

			this.mItemsShown = (uint) System.Math.Max(2, System.Math.Min(this.mMaxItemsShown, this.mItems.Count));

			for (int i = 0; i < this.mItemsShown; i++)   // create all the item elements
			{
				BorderPanelOverlayElement e = (BorderPanelOverlayElement) OverlayManager.Singleton.CreateOverlayElementFromTemplate("SdkTrays/SelectMenuItem", "BorderPanel",this.mExpandedBox.Name + "/Item" + (i + 1));

				e.Top = 6 + i * (this.mSmallBox.Height - 8);
				e.Width = this.mExpandedBox.Width - 32;

				this.mExpandedBox._addChild(e);
				this.mItemElements.Add(e);
			}

			if (items.Count != 0) { this.selectItem(0, false); }
			else { this.mSmallTextArea.Caption = ""; }
		}

		public void addItem(string item)
		{
			this.mItems.Add(item);
			this.setItems(this.mItems);
		}

		public void removeItem(string item)
		{
			int it = 0;

			for (; it < this.mItems.Count; it++)
				if (item == this.mItems[it]) { break; }

			if (it < this.mItems.Count)
			{
				this.mItems.RemoveAt(it);
				if (this.mItems.Count < this.mItemsShown)
				{
					this.mItemsShown = (uint) this.mItems.Count;
					nukeOverlayElement(this.mItemElements[this.mItemElements.Count - 1]);
					this.mItemElements.RemoveAt(this.mItemElements.Count - 1);
				}
			}
			else 
                throw new Exception("Menu \"" + this.getName() + "\" contains no item \"" + item + "\".");
		}

		public void removeItem(uint index)
		{
			int it = 0;
			uint i = 0;

			for (; it < this.mItems.Count; it++)
			{
				if (i == index) break;
				i++;
			}

			if (it < this.mItems.Count)
			{
				this.mItems.RemoveAt(it);
				if (this.mItems.Count < this.mItemsShown)
				{
					this.mItemsShown = (uint)this.mItems.Count;
					nukeOverlayElement(this.mItemElements[this.mItemElements.Count - 1]);
					this.mItemElements.RemoveAt(this.mItemElements.Count - 1);
				}
			}
			else 
                throw new Exception("Menu \"" + this.getName() + "\" contains no item at position " + index + ".");
		}

		public void clearItems()
		{
			this.mItems.Clear();
			this.mSelectionIndex = -1;
			this.mSmallTextArea.Caption = "";
		}

		public void selectItem(uint index, bool notifyListener = true)
		{
			if (index >= mItems.Count)
				throw new Exception("Menu \"" + this.getName() + "\" contains no item at position " + index + ".");

			this.mSelectionIndex = (int) index;
			fitCaptionToArea(this.mItems[(int)index], this.mSmallTextArea, this.mSmallBox.Width - this.mSmallTextArea.Left * 2);

			if (this.mListener != null && notifyListener) this.mListener.itemSelected(this);
		}

		public void selectItem(string item, bool notifyListener = true)
		{
			for (int i = 0; i < this.mItems.Count; i++)
			{
				if (item == this.mItems[i])
				{
					this.selectItem((uint)i, notifyListener);
					return;
				}
			}

			throw new Exception("Menu \"" + this.getName() + "\" contains no item \"" + item + "\".");
		}

		public string getSelectedItem()
		{
		    if (this.mSelectionIndex != -1) { return this.mItems[mSelectionIndex]; }

		    throw new Exception("Menu \"" + this.getName() + "\" has no item selected.");
		}

	    public int getSelectionIndex() { return this.mSelectionIndex; }

		public override void _cursorPressed(Vector2 cursorPos)
		{
			OverlayManager om = OverlayManager.Singleton;

			if (this.mExpanded)
			{
				if (this.mScrollHandle.IsVisible)   // check for scrolling
				{
					Vector2 co = Widget.cursorOffset(this.mScrollHandle, cursorPos);

					if (co.SquaredLength <= 81)
					{
						this.mDragging = true;
						this.mDragOffset = co.y;
						return;
					}
					
                    if (Widget.isCursorOver(this.mScrollTrack, cursorPos))
					{
						float newTop = this.mScrollHandle.Top + co.y;
						float lowerBoundary = this.mScrollTrack.Height - this.mScrollHandle.Height;
						this.mScrollHandle.Top = MathHelper.clamp((int)newTop, 0, (int)lowerBoundary);

						float scrollPercentage = MathHelper.clamp(newTop / lowerBoundary, 0, 1);
						this.setDisplayIndex((int) (scrollPercentage * (this.mItems.Count - this.mItemElements.Count) + 0.5));
						return;
					}
				}

				if (!isCursorOver(this.mExpandedBox, cursorPos, 3)) { this.retract(); }
				else
				{
					float l = this.mItemElements[0]._getDerivedLeft() * om.ViewportWidth + 5;
					float t = this.mItemElements[0]._getDerivedTop() * om.ViewportHeight + 5;
					float r = l + this.mItemElements[this.mItemElements.Count - 1].Width - 10;
					float b = this.mItemElements[this.mItemElements.Count - 1]._getDerivedTop() * om.ViewportHeight +
						      this.mItemElements[this.mItemElements.Count - 1].Height - 5;

					if (cursorPos.x >= l && cursorPos.x <= r && cursorPos.y >= t && cursorPos.y <= b)
					{
						if (this.mHighlightIndex != this.mSelectionIndex) { this.selectItem((uint)this.mHighlightIndex); }
						this.retract();
					}
				}
			}
			else
			{
				if (this.mItems.Count < 2) return;   // don't waste time showing a menu if there's no choice

				if (isCursorOver(this.mSmallBox, cursorPos, 4))
				{
					this.mExpandedBox.Show();
					this.mSmallBox.Hide();

					// calculate how much vertical space we need
					float idealHeight = this.mItemsShown * (this.mSmallBox.Height - 8) + 20;
					this.mExpandedBox.Height = idealHeight;
					this.mScrollTrack.Height = this.mExpandedBox.Height - 20;

					this.mExpandedBox.Left = this.mSmallBox.Left - 4;

					// if the expanded menu goes down off the screen, make it go up instead
					if (this.mSmallBox._getDerivedTop() * om.ViewportHeight + idealHeight > om.ViewportHeight)
					{
						this.mExpandedBox.Top = this.mSmallBox.Top + this.mSmallBox.Height - idealHeight + 3;
						// if we're in thick style, hide the caption because it will interfere with the expanded menu
						if (this.mTextArea.HorizontalAlignment == GuiHorizontalAlignment.GHA_CENTER) { this.mTextArea.Hide(); }
					}
					else { this.mExpandedBox.Top = this.mSmallBox.Top + 3; }

					this.mExpanded = true;
					this.mHighlightIndex = this.mSelectionIndex;
					this.setDisplayIndex(this.mHighlightIndex);

					if (this.mItemsShown < this.mItems.Count)  // update scrollbar position
					{
						this.mScrollHandle.Show();
						float lowerBoundary = this.mScrollTrack.Height - this.mScrollHandle.Height;
						this.mScrollHandle.Top = (int)(mDisplayIndex * lowerBoundary / (mItems.Count - this.mItemElements.Count));
					}
					else this.mScrollHandle.Hide();
				}
			}
		}

		public override void _cursorReleased(Vector2 cursorPos) { this.mDragging = false; }

		public override void _cursorMoved(Vector2 cursorPos)
		{
			OverlayManager om = OverlayManager.Singleton;

			if (this.mExpanded)
			{
				if (this.mDragging)
				{
					Vector2 co = Widget.cursorOffset(this.mScrollHandle, cursorPos);
					float newTop = this.mScrollHandle.Top + co.y - this.mDragOffset;
					float lowerBoundary = this.mScrollTrack.Height - this.mScrollHandle.Height;
					this.mScrollHandle.Top = MathHelper.clamp((int)newTop, 0, (int)lowerBoundary);

					float scrollPercentage = MathHelper.clamp(newTop / lowerBoundary, 0, 1);
					int newIndex = (int) (scrollPercentage * (this.mItems.Count - this.mItemElements.Count) + 0.5);
					if (newIndex != this.mDisplayIndex) { this.setDisplayIndex(newIndex); }
					return;
				}

				float l = this.mItemElements[0]._getDerivedLeft() * om.ViewportWidth + 5;
				float t = this.mItemElements[0]._getDerivedTop() * om.ViewportHeight + 5;
				float r = l + this.mItemElements[this.mItemElements.Count - 1].Width - 10;
				float b = this.mItemElements[this.mItemElements.Count - 1]._getDerivedTop() * om.ViewportHeight +
					this.mItemElements[this.mItemElements.Count - 1].Height - 5;

				if (cursorPos.x >= l && cursorPos.x <= r && cursorPos.y >= t && cursorPos.y <= b)
				{
					int newIndex = (int)(this.mDisplayIndex + (cursorPos.y - t) / (b - t) * this.mItemElements.Count);
					if (this.mHighlightIndex != newIndex)
					{
						this.mHighlightIndex = newIndex;
						this.setDisplayIndex(this.mDisplayIndex);
					}
				}
			}
			else
			{
				if (isCursorOver(mSmallBox, cursorPos, 4))
				{
					this.mSmallBox.MaterialName = "SdkTrays/MiniTextBox/Over";
					this.mSmallBox.BorderMaterialName = "SdkTrays/MiniTextBox/Over";
					this.mCursorOver = true;
				}
				else
				{
					if (this.mCursorOver)
					{
						this.mSmallBox.MaterialName = "SdkTrays/MiniTextBox";
						this.mSmallBox.BorderMaterialName = "SdkTrays/MiniTextBox";
						this.mCursorOver = false;
					}
				}
			}
		}

		public override void _focusLost() { if (this.mExpandedBox.IsVisible) { this.retract(); } }

		/*-----------------------------------------------------------------------------
		| Internal method - sets which item goes at the top of the expanded menu.
		-----------------------------------------------------------------------------*/
		protected void setDisplayIndex(int index)
		{
			index = System.Math.Min(index, this.mItems.Count - this.mItemElements.Count);
			this.mDisplayIndex = index;
			BorderPanelOverlayElement ie;
			TextAreaOverlayElement ta;

			for (int i = 0; i < this.mItemElements.Count; i++)
			{
				ie = this.mItemElements[i];
				ta = (TextAreaOverlayElement)ie.GetChild(ie.Name + "/MenuItemText");

				fitCaptionToArea(this.mItems[this.mDisplayIndex + i], ta, ie.Width - 2 * ta.Left);

				if ((mDisplayIndex + i) == this.mHighlightIndex)
				{
					ie.MaterialName = "SdkTrays/MiniTextBox/Over";
					ie.BorderMaterialName = "SdkTrays/MiniTextBox/Over";
				}
				else
				{
					ie.MaterialName = "SdkTrays/MiniTextBox";
					ie.BorderMaterialName = "SdkTrays/MiniTextBox";
				}
			}
		}

		/*-----------------------------------------------------------------------------
		| Internal method - cleans up an expanded menu.
		-----------------------------------------------------------------------------*/
		internal void retract()
		{
			this.mDragging = false;
			this.mExpanded = false;
			this.mExpandedBox.Hide();
			this.mTextArea.Show();
			this.mSmallBox.Show();
			this.mSmallBox.MaterialName = "SdkTrays/MiniTextBox";
			this.mSmallBox.BorderMaterialName = "SdkTrays/MiniTextBox";
		}
	}

	/*=============================================================================
	| Basic label widget.
	=============================================================================*/
	public class Label : Widget {
        protected TextAreaOverlayElement mTextArea;
		protected bool mFitToTray;

		// Do not instantiate any widgets directly. Use SdkTrayManager.
		public Label(String name, string caption, float width)
		{
			this.mElement = OverlayManager.Singleton.CreateOverlayElementFromTemplate("SdkTrays/Label", "BorderPanel", name);
			this.mTextArea = (TextAreaOverlayElement)((OverlayContainer)this.mElement).GetChild(this.getName() + "/LabelCaption");
			this.setCaption(caption);
			if (width <= 0) mFitToTray = true;
			else {
				this.mFitToTray = false;
				this.mElement.Width = width;
			}
		}

		public string getCaption() { return this.mTextArea.Caption; }
		public void setCaption(string caption) { this.mTextArea.Caption = caption; }
		public override void _cursorPressed(Vector2 cursorPos) { if (this.mListener != null && isCursorOver(this.mElement, cursorPos, 3)) this.mListener.labelHit(this); }
		public bool _isFitToTray() { return mFitToTray; }

	}

	/*=============================================================================
	| Basic separator widget.
	=============================================================================*/
	public class Separator : Widget {

        protected bool mFitToTray;
		// Do not instantiate any widgets directly. Use SdkTrayManager.
        public Separator(String name, float width) {
			this.mElement = OverlayManager.Singleton.CreateOverlayElementFromTemplate("SdkTrays/Separator", "Panel", name);
			if (width <= 0) mFitToTray = true;
			else {
				mFitToTray = false;
				this.mElement.Width = width;
			}
		}

		public bool _isFitToTray() { return mFitToTray; }

	}

	/*=============================================================================
	| Basic slider widget.
	=============================================================================*/
    public class Slider : Widget {
		protected TextAreaOverlayElement mTextArea;
		protected TextAreaOverlayElement mValueTextArea;
		protected BorderPanelOverlayElement mTrack;
		protected PanelOverlayElement mHandle;
		protected bool mDragging;
		protected bool mFitToContents;
		protected float mDragOffset;
		protected float mValue;
		protected float mMinValue;
		protected float mMaxValue;
		protected float mInterval;
	
		// Do not instantiate any widgets directly. Use SdkTrayManager.
		public Slider(string name, string caption, float width, float trackWidth, float valueBoxWidth, float minValue, float maxValue, uint snaps)
		{
			this.mDragOffset = 0;
		    this.mValue = 0;
		    this.mMinValue = 0;
		    this.mMaxValue = 0;
		    this.mInterval = 0;
            this.mDragging = false;
			this.mFitToContents = false;
			this.mElement = OverlayManager.Singleton.CreateOverlayElementFromTemplate("SdkTrays/Slider", "BorderPanel", name);
			this.mElement.Width = width;
			OverlayContainer c = (OverlayContainer)this.mElement;
			this.mTextArea = (TextAreaOverlayElement)c.GetChild(this.getName() + "/SliderCaption");
			OverlayContainer valueBox = (OverlayContainer)c.GetChild(this.getName() + "/SliderValueBox");
			valueBox.Width = valueBoxWidth;
			valueBox.Left = -(valueBoxWidth + 5);
			this.mValueTextArea = (TextAreaOverlayElement)valueBox.GetChild(valueBox.Name + "/SliderValueText");
			this.mTrack = (BorderPanelOverlayElement)c.GetChild(this.getName() + "/SliderTrack");
			this.mHandle = (PanelOverlayElement)this.mTrack.GetChild(this.mTrack.Name + "/SliderHandle");
            
			if (trackWidth <= 0)  // tall style
			{
				this.mTrack.Width = width - 16;
			}
			else  // long style
			{
				if (width <= 0) this.mFitToContents = true;
				this.mElement.Height = 34;
				this.mTextArea.Top = 10;
				valueBox.Top = 2;
				this.mTrack.Top = -23;
				this.mTrack.Width = trackWidth;
				this.mTrack.HorizontalAlignment = GuiHorizontalAlignment.GHA_RIGHT;
				this.mTrack.Left = -(trackWidth + valueBoxWidth + 5);
			}

			this.setCaption(caption);
			this.setRange(minValue, maxValue, snaps, false);
		}

		/*-----------------------------------------------------------------------------
		| Sets the minimum value, maximum value, and the number of snapping points.
		-----------------------------------------------------------------------------*/
		public void setRange(float minValue, float maxValue, uint snaps, bool notifyListener = true)
		{
			this.mMinValue = minValue;
			this.mMaxValue = maxValue;

			if (snaps <= 1 || this.mMinValue >= this.mMaxValue)
			{
				this.mInterval = 0;
				this.mHandle.Hide();
				this.mValue = minValue;
				this.mValueTextArea.Caption = snaps == 1 ? this.mMinValue.ToString() : "";
			}
			else
			{
				this.mHandle.Show();
				this.mInterval = (maxValue - minValue) / (snaps - 1);
				setValue(minValue, notifyListener);
			}
		}

		public string getValueCaption() { return this.mValueTextArea.Caption; }
		
		/*-----------------------------------------------------------------------------
		| You can use this method to manually format how the value is displayed.
		-----------------------------------------------------------------------------*/
		public void setValueCaption(string caption) { this.mValueTextArea.Caption = caption; }

		public void setValue(float value, bool notifyListener = true)
		{
			if (this.mInterval == 0) { return; }

			this.mValue = MathHelper.clamp(value, this.mMinValue, this.mMaxValue);

			this.setValueCaption(this.mValue.ToString());

			if (this.mListener != null && notifyListener) { this.mListener.sliderMoved(this); }

		    if (!this.mDragging)
                this.mHandle.Left = (int) ((this.mValue - this.mMinValue)/(this.mMaxValue - this.mMinValue) * (this.mTrack.Width - this.mHandle.Width));
		}

		public float getValue() { return this.mValue; }

		public string getCaption() { return this.mTextArea.Caption; }

		public void setCaption(string caption)
		{
			this.mTextArea.Caption = caption;

			if (this.mFitToContents)
                this.mElement.Width = getCaptionWidth(caption, this.mTextArea) + this.mValueTextArea.Parent.Width + this.mTrack.Width + 26;
		}

		public override void _cursorPressed(Vector2 cursorPos)
		{
			if (!this.mHandle.IsVisible) { return; }

			Vector2 co = Widget.cursorOffset(this.mHandle, cursorPos);

			if (co.SquaredLength <= 81)
			{
				this.mDragging = true;
				this.mDragOffset = co.x;
			}
			else if (Widget.isCursorOver(this.mTrack, cursorPos))
			{
				float newLeft = this.mHandle.Left + co.x;
				float rightBoundary = this.mTrack.Width - this.mHandle.Width;

				this.mHandle.Left = MathHelper.clamp((int)newLeft, 0, (int)rightBoundary);
				this.setValue(getSnappedValue(newLeft / rightBoundary));
			}
		}

		public override void _cursorReleased(Vector2 cursorPos)
		{
			if (this.mDragging)
			{
				this.mDragging = false;
				this.mHandle.Left = (int)((this.mValue - this.mMinValue) / (this.mMaxValue - this.mMinValue) * 
					(this.mTrack.Width - this.mHandle.Width));
			}
		}

		public override void _cursorMoved(Vector2 cursorPos)
		{
			if (this.mDragging)
			{
				Vector2 co = Widget.cursorOffset(this.mHandle, cursorPos);
				float newLeft = this.mHandle.Left + co.x - this.mDragOffset;
				float rightBoundary = this.mTrack.Width - this.mHandle.Width;

				this.mHandle.Left = MathHelper.clamp((int)newLeft, 0, (int)rightBoundary);
				setValue(getSnappedValue(newLeft / rightBoundary));
			}
		}

		public override void _focusLost()  { this.mDragging = false; }

		/*-----------------------------------------------------------------------------
		| Internal method - given a percentage (from left to right), gets the
		| value of the nearest marker.
		-----------------------------------------------------------------------------*/
		protected float getSnappedValue(float percentage)
		{
			percentage = MathHelper.clamp(percentage, 0, 1);
			uint whichMarker = (uint) (percentage * (this.mMaxValue - this.mMinValue) / this.mInterval + 0.5);
			return whichMarker * this.mInterval + this.mMinValue;
		}
	}

	/*=============================================================================
	| Basic parameters panel widget.
	=============================================================================*/
	public class ParamsPanel : Widget {

        protected TextAreaOverlayElement mNamesArea;
		protected TextAreaOverlayElement mValuesArea;
		protected string[] mNames;
		protected string[] mValues;

		// Do not instantiate any widgets directly. Use SdkTrayManager.
		public ParamsPanel(String name, float width, int lines) {
			this.mElement = OverlayManager.Singleton.CreateOverlayElementFromTemplate ("SdkTrays/ParamsPanel", "BorderPanel", name);
			OverlayContainer c = (OverlayContainer)this.mElement;
			this.mNamesArea =  (TextAreaOverlayElement)c.GetChild(this.getName() + "/ParamsPanelNames");
			this.mValuesArea = (TextAreaOverlayElement)c.GetChild(this.getName() + "/ParamsPanelValues");
			this.mElement.Width = width;
			this.mElement.Height = this.mNamesArea.Top * 2 + lines * this.mNamesArea.CharHeight;

            this.mValues = new string[0];
            this.mNames  = new string[0];
		}

		public void setAllParamNames(string[] paramNames) {
			this.mNames = paramNames;
			this.mValues = new string[0];
			Array.Resize<string>(ref this.mValues, mNames.Length);
			this.mElement.Height = this.mNamesArea.Top * 2 + mNames.Length * this.mNamesArea.CharHeight;
			this.updateText();
		}

        public string[] getAllParamNames() { return mNames; }

        public void setAllParamValues(string[] paramValues) {
			this.mValues = paramValues;
            Array.Resize<string>(ref this.mValues, mNames.Length);
			this.updateText();
		}

		public void setParamValue(string paramName, string paramValue) {
			for (int i = 0; i < this.mNames.Length; i++) {
				if (mNames[i] == paramName) {
					mValues[i] = paramValue;
					this.updateText();
					return;
				}
			}
			String desc = "ParamsPanel \"" + getName() + "\" has no parameter \"" + paramName + "\".";
			throw new Exception("Item not found : " + desc + " ParamsPanel.setParamValue");
		}

		public void setParamValue(int index, string paramValue) {
            if(index >= this.mNames.Length) {
				String desc = "ParamsPanel \"" + getName() + "\" has no parameter at position " +
					index.ToString() + ".";
				throw new Exception("Item not found : " + desc + "ParamsPanel.setParamValue");
			}

			this.mValues[index] = paramValue;
			this.updateText();
		}

		public string getParamValue(string paramName) {
            for(int i = 0; i < mNames.Length; i++) {
				if (mNames[i] == paramName) return mValues[i];
			}
			
			String desc = "ParamsPanel \"" + getName() + "\" has no parameter \"" + paramName + "\".";
			throw new Exception("Item not found : " + desc + "ParamsPanel.getParamValue");
		}

		public string getParamValue(int index) {
            if(index >= this.mNames.Length) {
				String desc = "ParamsPanel \"" + getName() + "\" has no parameter at position " +
					index.ToString() + ".";
				throw new Exception("Item not found : " + desc + "ParamsPanel.getParamValue");
			}
			return mValues[index];
		}

		public string[] getAllParamValues() { return mValues; }


		/*-----------------------------------------------------------------------------
		| Internal method - updates text areas based on name and value lists.
		-----------------------------------------------------------------------------*/
		protected void updateText() {
			string namesDS = "";
			string valuesDS = "";

			for (int i = 0; i < mNames.Length; i++) {
				namesDS += mNames[i] + ":\n";
				valuesDS += mValues[i] + "\n";
			}

			mNamesArea.Caption = namesDS;
			mValuesArea.Caption = valuesDS;
		}
	}

	/*=============================================================================
	| Basic check box widget.
	=============================================================================*/
	public class CheckBox : Widget {

        protected TextAreaOverlayElement mTextArea;
		protected BorderPanelOverlayElement mSquare;
		protected OverlayElement mX;
		protected bool mFitToContents;
		protected bool mCursorOver;

		// Do not instantiate any widgets directly. Use SdkTrayManager.
		public CheckBox(string name, string caption, float width) {
			this.mCursorOver = false;
			this.mFitToContents = width <= 0;
			this.mElement = OverlayManager.Singleton.CreateOverlayElementFromTemplate("SdkTrays/CheckBox", "BorderPanel", name);
			OverlayContainer c = (OverlayContainer)mElement;
			this.mTextArea = (TextAreaOverlayElement)c.GetChild(getName() + "/CheckBoxCaption");
			this.mSquare = (BorderPanelOverlayElement)c.GetChild(getName() + "/CheckBoxSquare");
			this.mX = this.mSquare.GetChild(this.mSquare.Name + "/CheckBoxX");
			this.mX.Hide();
			this.mElement.Width = width;
			this.setCaption(caption);
		}

		public string getCaption() { return mTextArea.Caption; }

		public void setCaption(string caption) {
			this.mTextArea.Caption = caption;
			if (mFitToContents) this.mElement.Width = getCaptionWidth(caption, mTextArea) + mSquare.Width + 23;
		}

		public bool isChecked() { return mX.IsVisible; }

		public void setChecked(bool chkd, bool notifyListener = true) {
			if (chkd) mX.Show();
			else mX.Hide();
			if (this.mListener != null && notifyListener) this.mListener.checkBoxToggled(this);
		}

		public void toggle(bool notifyListener = true) { this.setChecked(!isChecked(), notifyListener); }

		public override void _cursorPressed(Vector2 cursorPos) { if (this.mCursorOver && this.mListener != null) toggle(); }

		public override void _cursorMoved(Vector2 cursorPos) {
			if (isCursorOver(this.mSquare, cursorPos, 5)) {
				if (!mCursorOver) {
					this.mCursorOver = true;
					this.mSquare.MaterialName = "SdkTrays/MiniTextBox/Over";
					this.mSquare.BorderMaterialName = "SdkTrays/MiniTextBox/Over";
				}
			}
			else {
				if (this.mCursorOver) {
					this.mCursorOver = false;
					this.mSquare.MaterialName = "SdkTrays/MiniTextBox";
					this.mSquare.BorderMaterialName = "SdkTrays/MiniTextBox";
				}
			}
		}

		public override void _focusLost() {
			this.mSquare.MaterialName = "SdkTrays/MiniTextBox";
			this.mSquare.BorderMaterialName = "SdkTrays/MiniTextBox";
			mCursorOver = false;
		}
	}

	/*=============================================================================
	| Custom, decorative widget created from a template.
	=============================================================================*/
	public class DecorWidget : Widget {

		// Do not instantiate any widgets directly. Use SdkTrayManager.
		public DecorWidget(string name, string templateName) {
			this.mElement = OverlayManager.Singleton.CreateOverlayElementFromTemplate(templateName, "", name);
		}
	}

	/*=============================================================================
	| Basic progress bar widget.
	=============================================================================*/
	public class ProgressBar : Widget
	{
		protected TextAreaOverlayElement mTextArea;
		protected TextAreaOverlayElement mCommentTextArea;
		protected OverlayElement mMeter;
		protected OverlayElement mFill;
		protected float mProgress;
        
        // Do not instantiate any widgets directly. Use SdkTrayManager.
		public ProgressBar(string name, string caption, float width, float commentBoxWidth)
		{
		    mProgress = 0;
            mElement = OverlayManager.Singleton.CreateOverlayElementFromTemplate("SdkTrays/ProgressBar", "BorderPanel", name);
			mElement.Width = width;
			OverlayContainer c = (OverlayContainer)mElement;
			mTextArea = (TextAreaOverlayElement)c.GetChild(getName() + "/ProgressCaption");
			OverlayContainer commentBox = (OverlayContainer)c.GetChild(getName() + "/ProgressCommentBox");
			commentBox.Width = commentBoxWidth;
			commentBox.Left = -(commentBoxWidth + 5);
			mCommentTextArea = (TextAreaOverlayElement)commentBox.GetChild(commentBox.Name + "/ProgressCommentText");
			mMeter = c.GetChild(getName() + "/ProgressMeter");
			mMeter.Width = width - 10;
			mFill = ((OverlayContainer)mMeter).GetChild(mMeter.Name + "/ProgressFill");
			setCaption(caption);
		}

		/*-----------------------------------------------------------------------------
		| Sets the progress as a percentage.
		-----------------------------------------------------------------------------*/
		public void setProgress(float progress)
		{
			mProgress = MathHelper.clamp(progress, 0, 1);
			mFill.Width = System.Math.Min((int)mFill.Height, (int)(mProgress * (mMeter.Width - 2 * mFill.Left)));
		}

		/*-----------------------------------------------------------------------------
		| Gets the progress as a percentage.
		-----------------------------------------------------------------------------*/
		public float getProgress()
		{
			return mProgress;
		}

		public string getCaption()
		{
			return mTextArea.Caption;
		}

		public void setCaption(string caption)
		{
			mTextArea.Caption = caption;
		}

		public string getComment()
		{
			return mCommentTextArea.Caption;
		}

		public void setComment(string comment)
		{
			mCommentTextArea.Caption = comment;
		}
	}

	/*=============================================================================
	| Main class to manage a cursor, backdrop, trays and widgets.
	=============================================================================*/
	public class SdkTrayManager : SdkTrayListener/*, ResourceGroupListener*/ {

        protected String mName;                   // name of this tray system
        protected RenderWindow mWindow;          // render window
        protected MOIS.Mouse mMouse;                   // mouse device
        protected Overlay mBackdropLayer;        // backdrop layer
        protected Overlay mTraysLayer;           // widget layer
        protected Overlay mPriorityLayer;        // top priority layer
        protected Overlay mCursorLayer;          // cursor layer
        protected OverlayContainer mBackdrop;    // backdrop
        protected OverlayContainer[] mTrays = new OverlayContainer[10];   // widget trays
        protected List<Widget>[] mWidgets = new List<Widget>[10];              // widgets
        protected List<Widget> mWidgetDeathRow = new List<Widget>();           // widget queue for deletion
        protected OverlayContainer mCursor;      // cursor
        protected SdkTrayListener mListener;           // tray listener
        protected float mWidgetPadding;            // widget padding
        protected float mWidgetSpacing;            // widget spacing
        protected float mTrayPadding;              // tray padding
		protected bool mTrayDrag;                       // a mouse press was initiated on a tray
        protected SelectMenu mExpandedMenu;            // top priority expanded menu widget
        protected TextBox mDialog;                     // top priority dialog widget
        protected OverlayContainer mDialogShade; // top priority dialog shade
        protected Button mOk;                          // top priority OK button
        protected Button mYes;                         // top priority Yes button
        protected Button mNo;                          // top priority No button
        protected bool mCursorWasVisible;               // cursor state before showing dialog
        protected Label mFpsLabel;                     // FPS label
        protected ParamsPanel mStatsPanel;             // frame stats panel
        protected DecorWidget mLogo;                   // logo
        protected ProgressBar mLoadBar;                // loading bar
        protected float mGroupInitProportion;      // proportion of load job assigned to initialising one resource group
        protected float mGroupLoadProportion;      // proportion of load job assigned to loading one resource group
        protected float mLoadInc;                  // loading increment
        protected GuiHorizontalAlignment[] mTrayWidgetAlign = new GuiHorizontalAlignment[10];   // tray widget alignments


		/*-----------------------------------------------------------------------------
		| Creates backdrop, cursor, and trays.
		-----------------------------------------------------------------------------*/
		public SdkTrayManager(String name, RenderWindow window, MOIS.Mouse mouse, SdkTrayListener listener = null) {
            this.mName = name;       this.mWindow = window;     this.mMouse = mouse;     this.mListener = listener; this.mWidgetPadding = 8;
            this.mWidgetSpacing = 2; this.mExpandedMenu = null; this.mDialog = null;     this.mOk = null;           this.mYes = null;
            this.mNo = null;         this.mFpsLabel = null;     this.mStatsPanel = null; this.mLogo = null;         this.mLoadBar = null;
			
            OverlayManager om = OverlayManager.Singleton;

			String nameBase = mName + "/";
            nameBase.Replace(' ', '_');

			// create overlay layers for everything

			this.mBackdropLayer = om.Create(nameBase + "BackdropLayer");
			this.mTraysLayer    = om.Create(nameBase + "WidgetsLayer");
			this.mPriorityLayer = om.Create(nameBase + "PriorityLayer");
			this.mCursorLayer   = om.Create(nameBase + "CursorLayer");

			this.mBackdropLayer.ZOrder = 100;
			this.mTraysLayer.ZOrder    = 200;
			this.mPriorityLayer.ZOrder = 300;
			this.mCursorLayer.ZOrder   = 400;

			// make backdrop and cursor overlay containers

			this.mCursor = (OverlayContainer)om.CreateOverlayElementFromTemplate("SdkTrays/Cursor", "Panel", nameBase + "Cursor");
			this.mCursorLayer.Add2D(mCursor);
			this.mBackdrop = (OverlayContainer)om.CreateOverlayElement("Panel", nameBase + "Backdrop");
			this.mBackdropLayer.Add2D(mBackdrop);
			this.mDialogShade = (OverlayContainer)om.CreateOverlayElement("Panel", nameBase + "DialogShade");
			this.mDialogShade.MaterialName = "SdkTrays/Shade";
			this.mDialogShade.Hide();
			this.mPriorityLayer.Add2D(mDialogShade);

            for(int i = 0; i < this.mWidgets.Length; i++)
                this.mWidgets[i] = new List<Widget>();

			String[] trayNames = new String[] { "TopLeft", "Top", "TopRight", "Left", "Center", "Right", "BottomLeft", "Bottom", "BottomRight" };

			for (int i = 0; i < 9; i++)    // make the float trays
			{
				this.mTrays[i] = (OverlayContainer)om.CreateOverlayElementFromTemplate ("SdkTrays/Tray", "BorderPanel", nameBase + trayNames[i] + "Tray");
				this.mTraysLayer.Add2D(mTrays[i]);

				this.mTrayWidgetAlign[i] = GHA.GHA_CENTER;

				// align trays based on location
				if (i == (int)TL.TL_TOP        || i == (int)TL.TL_CENTER || i == (int)TL.TL_BOTTOM)      this.mTrays[i].HorizontalAlignment = GHA.GHA_CENTER;
				if (i == (int)TL.TL_LEFT       || i == (int)TL.TL_CENTER || i == (int)TL.TL_RIGHT)       this.mTrays[i].VerticalAlignment   = GuiVerticalAlignment.GVA_CENTER;
				if (i == (int)TL.TL_TOPRIGHT   || i == (int)TL.TL_RIGHT  || i == (int)TL.TL_BOTTOMRIGHT) this.mTrays[i].HorizontalAlignment = GHA.GHA_RIGHT;
				if (i == (int)TL.TL_BOTTOMLEFT || i == (int)TL.TL_BOTTOM || i == (int)TL.TL_BOTTOMRIGHT) this.mTrays[i].VerticalAlignment   = GuiVerticalAlignment.GVA_BOTTOM;
			}

			// create the null tray for free-floating widgets
			this.mTrays[9] = (OverlayContainer)om.CreateOverlayElement("Panel", nameBase + "NullTray");
			this.mTrayWidgetAlign[9] = GHA.GHA_LEFT;
			this.mTraysLayer.Add2D(mTrays[9]);

			this.adjustTrays();
			
			this.showTrays();
			this.showCursor();
		}

		/*-----------------------------------------------------------------------------
		| Destroys background, cursor, widgets, and trays.
		-----------------------------------------------------------------------------*/
        ~SdkTrayManager() {
			OverlayManager om = OverlayManager.Singleton;


            // delete widgets queued for destruction
			for (int i = 0; i < mWidgetDeathRow.Count; i++) { mWidgetDeathRow[i] = null; }
			mWidgetDeathRow.Clear();

			//this.closeDialog();
			//this.hideLoadingBar();

			//Widget.nukeOverlayElement(mBackdrop);
			//Widget.nukeOverlayElement(mCursor);
			//Widget.nukeOverlayElement(mDialogShade);

            om.DestroyAll();

			//for (int i = 0; i < 10; i++) { Widget.nukeOverlayElement(mTrays[i]); }
		}

		/*-----------------------------------------------------------------------------
		| Converts a 2D screen coordinate (in pixels) to a 3D ray into the scene.
		-----------------------------------------------------------------------------*/
        public static Ray screenToScene(Camera cam, Vector2 pt) { return cam.GetCameraToViewportRay(pt.x, pt.y); }

		/*-----------------------------------------------------------------------------
		| Converts a 3D scene position to a 2D screen coordinate (in pixels).
		-----------------------------------------------------------------------------*/
        public static Vector2 sceneToScreen(Camera cam, Vector3 pt) {
			Vector3 result = cam.ProjectionMatrix * cam.ViewMatrix * pt;
			return new Vector2((result.x + 1) / 2, -(result.y + 1) / 2);
		}

		// these methods get the underlying overlays and overlay elements

        public OverlayContainer getTrayContainer(TrayLocation trayLoc) { return this.mTrays[(int)trayLoc]; }
        public Overlay getBackdropLayer() { return this.mBackdropLayer; }
        public Overlay getTraysLayer() { return this.mTraysLayer; }
        public Overlay getCursorLayer() { return this.mCursorLayer; }
        public OverlayContainer getBackdropContainer() { return this.mBackdrop; }
        public OverlayContainer getCursorContainer() { return this.mCursor; }
        public OverlayElement getCursorImage() { return mCursor.GetChild(mCursor.Name + "/CursorImage"); }

        public void setListener(SdkTrayListener listener) { mListener = listener; }
        public SdkTrayListener getListener() { return mListener; }

        public void showAll() {
			showBackdrop();
			showTrays();
			showCursor();
		}

        public void hideAll() {
			hideBackdrop();
			hideTrays();
			hideCursor();
		}

		/*-----------------------------------------------------------------------------
		| Displays specified material on backdrop, or the last material used if
		| none specified. Good for pause menus like in the browser.
		-----------------------------------------------------------------------------*/
        public void showBackdrop(string materialName = "") {
			if (materialName != "") this.mBackdrop.MaterialName = materialName;
			mBackdropLayer.Show();
		}

        public void hideBackdrop() { this.mBackdropLayer.Hide(); }

		/*-----------------------------------------------------------------------------
		| Displays specified material on cursor, or the last material used if
		| none specified. Used to change cursor type.
		-----------------------------------------------------------------------------*/
        public void showCursor(string materialName = "") {
			if (materialName != "") getCursorImage().MaterialName =  materialName;

			if (!mCursorLayer.IsVisible) {
				this.mCursorLayer.Show();
				this.refreshCursor();
			}
		}

        public void hideCursor() {
			this.mCursorLayer.Hide();

			// give widgets a chance to reset in case they're in the middle of something
			for (int i = 0; i < 10; i++) { for (int j = 0; j < this.mWidgets[i].Count; j++) { this.mWidgets[i][j]._focusLost(); } }

			this.setExpandedMenu(null);
		}

		/*-----------------------------------------------------------------------------
		| Updates cursor position based on unbuffered mouse state. This is necessary
		| because if the tray manager has been cut off from mouse events for a time,
		| the cursor position will be out of date.
		-----------------------------------------------------------------------------*/
        public void refreshCursor() { this.mCursor.SetPosition(mMouse.MouseState.X.abs, mMouse.MouseState.Y.abs); }

        public void showTrays() {
			this.mTraysLayer.Show();
			this.mPriorityLayer.Show();
		}

        public void hideTrays() {
			this.mTraysLayer.Hide();
			this.mPriorityLayer.Hide();

			// give widgets a chance to reset in case they're in the middle of something
			for (int i = 0; i < 10; i++) { for (int j = 0; j < mWidgets[i].Count; j++) { this.mWidgets[i][j]._focusLost(); } }

			this.setExpandedMenu(null);
		}

        public bool isCursorVisible() { return this.mCursorLayer.IsVisible; }
        public bool isBackdropVisible() { return this.mBackdropLayer.IsVisible; }
        public bool areTraysVisible() { return this.mTraysLayer.IsVisible; }

		/*-----------------------------------------------------------------------------
		| Sets horizontal alignment of a tray's contents.
		-----------------------------------------------------------------------------*/
        public void setTrayWidgetAlignment(TrayLocation trayLoc, GuiHorizontalAlignment gha) {
			mTrayWidgetAlign[(int)trayLoc] = gha;

			for (int i = 0; i < mWidgets[(int)trayLoc].Count; i++) { mWidgets[(int)trayLoc][i].getOverlayElement().HorizontalAlignment = gha;}
		}

		// padding and spacing methods

        public void setWidgetPadding(float padding) {
			this.mWidgetPadding = System.Math.Max((int)padding, 0);
			this.adjustTrays();
		}

        public void setWidgetSpacing(float spacing) {
			this.mWidgetSpacing = System.Math.Max((int)spacing, 0);
			this.adjustTrays();
		}
        public void setTrayPadding(float padding) {
			this.mTrayPadding = System.Math.Max((int)padding, 0);
			this.adjustTrays();
		}

        public virtual float getWidgetPadding() { return this.mWidgetPadding; }
        public virtual float getWidgetSpacing() { return this.mWidgetSpacing; }
        public virtual float getTrayPadding() { return this.mTrayPadding; }

		/*-----------------------------------------------------------------------------
		| Fits trays to their contents and snaps them to their anchor locations.
		-----------------------------------------------------------------------------*/
        public virtual void adjustTrays() {
            // resizes and hides trays if necessary
			for (int i = 0; i < 9; i++) {
				float trayWidth = 0;
				float trayHeight = mWidgetPadding;
				List<OverlayElement> labelsAndSeps = new List<OverlayElement>();
                
                // hide tray if empty
				if (mWidgets[i].Count == 0) {
					this.mTrays[i].Hide();
					continue;
				}
			    this.mTrays[i].Show();
			    // arrange widgets and calculate final tray size and position
				for (int j = 0; j < mWidgets[i].Count; j++) {
					OverlayElement e = this.mWidgets[i][j].getOverlayElement();

					if (j != 0) trayHeight += mWidgetSpacing;   // don't space first widget

					e.VerticalAlignment = GuiVerticalAlignment.GVA_TOP;
					e.Top = trayHeight;

					switch (e.HorizontalAlignment) {
					    case GHA.GHA_LEFT:  e.Left = this.mWidgetPadding;              break;
					    case GHA.GHA_RIGHT: e.Left = -(e.Width + this.mWidgetPadding); break;
                        default: e.Left = -(e.Width / 2);                              break;
					}

					// prevents some weird texture filtering problems (just some)
					e.SetPosition  ((int)e.Left, (int)e.Top);
					e.SetDimensions((int)e.Width, (int)e.Height);
					trayHeight += e.Height;

				    if (mWidgets[i][j].GetType().IsInstanceOfType(typeof (Label))) {
				        Label l = (Label) mWidgets[i][j];
				        if (l != null && l._isFitToTray())
				        {
				            labelsAndSeps.Add(e);
				            continue;
				        }
				    }

				    if (mWidgets[i][j].GetType().IsInstanceOfType(typeof(Separator)))
				    {
				        Separator s = (Separator) mWidgets[i][j];
				        if (s != null && s._isFitToTray())
				        {
				            labelsAndSeps.Add(e);
				            continue;
				        }
				    }

				    if (e.Width > trayWidth) trayWidth = e.Width;
				}

				// add paddings and resize trays
				mTrays[i].Width  = trayWidth + 2 * mWidgetPadding;
				mTrays[i].Height = trayHeight + mWidgetPadding;

				for (int j = 0; j < labelsAndSeps.Count; j++) {
					labelsAndSeps[j].Width = (int)trayWidth;
					labelsAndSeps[j].Left  = -(int)(trayWidth / 2);
				}
			}
            // snap trays to anchors
			for (int i = 0; i < 9; i++) {
                
				if (i == (int)TrayLocation.TL_TOPLEFT    || i == (int)TrayLocation.TL_LEFT   || i == (int)TrayLocation.TL_BOTTOMLEFT)
					mTrays[i].Left = mTrayPadding;
				if (i == (int)TrayLocation.TL_TOP        || i == (int)TrayLocation.TL_CENTER || i == (int)TrayLocation.TL_BOTTOM)
					mTrays[i].Left = -mTrays[i].Width / 2;
				if (i == (int)TrayLocation.TL_TOPRIGHT   || i == (int)TrayLocation.TL_RIGHT  || i == (int)TrayLocation.TL_BOTTOMRIGHT)
					mTrays[i].Left = -(mTrays[i].Width + mTrayPadding);

				if (i == (int)TrayLocation.TL_TOPLEFT    || i == (int)TrayLocation.TL_TOP    || i == (int)TrayLocation.TL_TOPRIGHT)
					mTrays[i].Top = mTrayPadding;
				if (i == (int)TrayLocation.TL_LEFT       || i == (int)TrayLocation.TL_CENTER || i == (int)TrayLocation.TL_RIGHT)
					mTrays[i].Top = -mTrays[i].Height / 2;
				if (i == (int)TrayLocation.TL_BOTTOMLEFT || i == (int)TrayLocation.TL_BOTTOM || i == (int)TrayLocation.TL_BOTTOMRIGHT)
					mTrays[i].Top = -mTrays[i].Height - mTrayPadding;

				// prevents some weird texture filtering problems (just some)
				mTrays[i].SetPosition((int)mTrays[i].Left, (int)mTrays[i].Top);
				mTrays[i].SetDimensions((int)mTrays[i].Width, (int)mTrays[i].Height);
			}
		}

		/*-----------------------------------------------------------------------------
		| Returns a 3D ray into the scene that is directly underneath the cursor.
		-----------------------------------------------------------------------------*/
        public Ray getCursorRay(Camera cam) { return screenToScene(cam, new Vector2(mCursor._getLeft(), mCursor._getTop())); }

        public Button createButton(TrayLocation trayLoc, string name, string caption, float width = 0) {
			Button b = new Button(name, caption, width);
			this.moveWidgetToTray(b, trayLoc);
			b._assignListener(mListener);
			return b;
		}

        public TextBox createTextBox(TrayLocation trayLoc, string name, string caption, float width, float height) {
			TextBox tb = new TextBox(name, caption, width, height);
			this.moveWidgetToTray(tb, trayLoc);
			tb._assignListener(mListener);
			return tb;
		}

        public SelectMenu createThickSelectMenu(TrayLocation trayLoc, string name, string caption, float width, int maxItemsShown, List<string> items = null) {
			if(items == null) { items = new List<string>(); }
            SelectMenu sm = new SelectMenu(name, caption, width, 0, (uint)maxItemsShown);
			this.moveWidgetToTray(sm, trayLoc);
			sm._assignListener(mListener);
			if (items.Count != 0) sm.setItems(items);
			return sm;
		}

        public SelectMenu createLongSelectMenu(TrayLocation trayLoc, string name, string caption, float width, float boxWidth, int maxItemsShown, List<string> items = null) {
			if(items == null) { items = new List<string>(); }
            SelectMenu sm = new SelectMenu(name, caption, width, boxWidth, (uint)maxItemsShown);
			this.moveWidgetToTray(sm, trayLoc);
			sm._assignListener(mListener);
			if (items.Count != 0) sm.setItems(items);
			return sm;
		}

        public SelectMenu createLongSelectMenu(TrayLocation trayLoc, string name, string caption, float boxWidth, int maxItemsShown, List<string> items = null) {
			return createLongSelectMenu(trayLoc, name, caption, 0, boxWidth, maxItemsShown, items);
		}

        public Label createLabel(TrayLocation trayLoc, string name, string caption, float width = 0) {
			Label l = new Label(name, caption, width);
			this.moveWidgetToTray(l, trayLoc);
			l._assignListener(mListener);
			return l;
		}

        public Separator createSeparator(TrayLocation trayLoc, string name, float width = 0) {
			Separator s = new Separator(name, width);
			moveWidgetToTray(s, trayLoc);
			return s;
		}

        public Slider createThickSlider(TrayLocation trayLoc, string name, string caption,
			float width, float valueBoxWidth, float minValue, float maxValue, int snaps) {
			
            Slider s = new Slider(name, caption, width, 0, valueBoxWidth, minValue, maxValue, (uint)snaps);
			this.moveWidgetToTray(s, trayLoc);
			s._assignListener(mListener);
			return s;
		}

        public Slider createLongSlider(TrayLocation trayLoc, string name, string caption, float width,
			float trackWidth, float valueBoxWidth, float minValue, float maxValue, int snaps) {

			if (trackWidth <= 0) trackWidth = 1;
			Slider s = new Slider(name, caption, width, trackWidth, valueBoxWidth, minValue, maxValue, (uint)snaps);
			this.moveWidgetToTray(s, trayLoc);
			s._assignListener(mListener);
			return s;
		}

        public Slider createLongSlider(TrayLocation trayLoc, string name, string caption, float trackWidth, float valueBoxWidth, float minValue, float maxValue, int snaps) { return createLongSlider(trayLoc, name, caption, 0, trackWidth, valueBoxWidth, minValue, maxValue, snaps); }

        public ParamsPanel createParamsPanel(TrayLocation trayLoc, string name, float width, int lines) {
			ParamsPanel pp = new ParamsPanel(name, width, lines);
			this.moveWidgetToTray(pp, trayLoc);
			return pp;
		}

        public ParamsPanel createParamsPanel(TrayLocation trayLoc, string name, float width, string[] paramNames) {
			ParamsPanel pp = new ParamsPanel(name, width, paramNames.Length);
			pp.setAllParamNames(paramNames);
			this.moveWidgetToTray(pp, trayLoc);
			return pp;
		}

        public CheckBox createCheckBox(TrayLocation trayLoc, string name, string caption, float width = 0) {
			CheckBox cb = new CheckBox(name, caption, width);
			this.moveWidgetToTray(cb, trayLoc);
			cb._assignListener(mListener);
			return cb;
		}

        public DecorWidget createDecorWidget(TrayLocation trayLoc, string name, string templateName) {
			DecorWidget dw = new DecorWidget(name, templateName);
			this.moveWidgetToTray(dw, trayLoc);
			return dw;
		}

        public ProgressBar createProgressBar(TrayLocation trayLoc, string name, string caption, float width, float commentBoxWidth) {
			ProgressBar pb = new ProgressBar(name, caption, width, commentBoxWidth);
			this.moveWidgetToTray(pb, trayLoc);
			return pb;
		}

		/*-----------------------------------------------------------------------------
		| Shows frame statistics widget set in the specified location.
		-----------------------------------------------------------------------------*/
        public void showFrameStats(TrayLocation trayLoc, int place = -1) {
			if (!areFrameStatsVisible()) {
				string[] stats = new string[] {"Average FPS", "Best FPS", "Worst FPS", "Triangles", "Batches"};

			    mFpsLabel = createLabel(TrayLocation.TL_NONE, mName + "/FpsLabel", "FPS:", 180);
				mFpsLabel._assignListener(this);
				mStatsPanel = createParamsPanel(TrayLocation.TL_NONE, mName + "/StatsPanel", 180, stats);
			}

			this.moveWidgetToTray(mFpsLabel, trayLoc, place);
			this.moveWidgetToTray(mStatsPanel, trayLoc, locateWidgetInTray(mFpsLabel) + 1);
		}

		/*-----------------------------------------------------------------------------
		| Hides frame statistics widget set.
		-----------------------------------------------------------------------------*/
        public void hideFrameStats() {
			if (areFrameStatsVisible()) {
				this.destroyWidget(mFpsLabel);
				this.destroyWidget(mStatsPanel);
				this.mFpsLabel = null;
				this.mStatsPanel = null;
			}
		}

        public bool areFrameStatsVisible() { return this.mFpsLabel != null; }

		/*-----------------------------------------------------------------------------
		| Toggles visibility of advanced statistics.
		-----------------------------------------------------------------------------*/
        public void toggleAdvancedFrameStats() { if(this.mFpsLabel != null) labelHit(this.mFpsLabel); }

		/*-----------------------------------------------------------------------------
		| Shows logo in the specified location.
		-----------------------------------------------------------------------------*/
        public void showLogo(TrayLocation trayLoc, int place = -1) {
			if (!isLogoVisible()) this.mLogo = this.createDecorWidget(TrayLocation.TL_NONE, this.mName + "/Logo", "SdkTrays/Logo");
			this.moveWidgetToTray(this.mLogo, trayLoc, place);
		}

        public void hideLogo() {
			if (isLogoVisible()) {
				this.destroyWidget(mLogo);
				this.mLogo = null;
			}
		}

        public bool isLogoVisible() { return this.mLogo != null; }

		/*-----------------------------------------------------------------------------
		| Shows loading bar. Also takes job settings: the number of resource groups
		| to be initialised, the number of resource groups to be loaded, and the
		| proportion of the job that will be taken up by initialisation. Usually,
		| script parsing takes up most time, so the default value is 0.7.
		-----------------------------------------------------------------------------*/
        public void showLoadingBar(int numGroupsInit = 1, int numGroupsLoad = 1, float initProportion = 0.7f) {
			if (this.mDialog  != null) closeDialog();
			if (this.mLoadBar != null) hideLoadingBar();

			this.mLoadBar = new ProgressBar(mName + "/LoadingBar", "Loading...", 400, 308);
			OverlayElement e = mLoadBar.getOverlayElement();
			mDialogShade.AddChild(e);
			e.VerticalAlignment = GuiVerticalAlignment.GVA_CENTER;
			e.Left = -(e.Width / 2);
			e.Top = -(e.Height / 2);

            //Ogre::ResourceGroupManager::getSingleton().addResourceGroupListener(this);
            //Has not been implemented ?

			this.mCursorWasVisible = this.isCursorVisible();
			this.hideCursor();
			this.mDialogShade.Show();

			// calculate the proportion of job required to init/load one group

			if (numGroupsInit == 0 && numGroupsLoad != 0) {
				this.mGroupInitProportion = 0;
				this.mGroupLoadProportion = 1;
			}
			else if (numGroupsLoad == 0 && numGroupsInit != 0) {
				this.mGroupLoadProportion = 0;
				if (numGroupsInit != 0) this.mGroupInitProportion = 1;
			}
			else if (numGroupsInit == 0 && numGroupsLoad == 0) {
				this.mGroupInitProportion = 0;
				this.mGroupLoadProportion = 0;
			}
			else {
				this.mGroupInitProportion = initProportion / numGroupsInit;
				this.mGroupLoadProportion = (1 - initProportion) / numGroupsLoad;
			}
		}

        public void hideLoadingBar()
		{
			if (this.mLoadBar != null) {
				this.mLoadBar.cleanup();
				this.mLoadBar = null;

				//ResourceGroupManager.Singleton.removeResourceGroupListener(this);
				if (this.mCursorWasVisible) this.showCursor();
				this.mDialogShade.Hide();
			}
		}

        public bool isLoadingBarVisible() {
            return mLoadBar != null;
        }

        /*-----------------------------------------------------------------------------
        | Pops up a message dialog with an OK button.
        -----------------------------------------------------------------------------*/
        public void showOkDialog(string caption, string message) {
            if(mLoadBar != null) hideLoadingBar();

            OverlayElement e;

            if(mDialog != null) {
                mDialog.setCaption(caption);
                mDialog.setText(message);

                if(mOk != null) return;

                mYes.cleanup();
                mNo.cleanup();
                mYes = null;
                mNo = null;
            } else {
                // give widgets a chance to reset in case they're in the middle of something
                for(int i = 0; i < 10; i++) {
                    for(int j = 0; j < mWidgets[i].Count; j++) {
                        mWidgets[i][j]._focusLost();
                    }
                }

                mDialogShade.Show();

                mDialog = new TextBox(mName + "/DialogBox", caption, 300, 208);
                mDialog.setText(message);
                e = mDialog.getOverlayElement();
                mDialogShade.AddChild(e);
                e.VerticalAlignment = GuiVerticalAlignment.GVA_CENTER;
                e.Left = -(e.Width / 2);
                e.Top = -(e.Height / 2);

                mCursorWasVisible = isCursorVisible();
                showCursor();
            }

            mOk = new Button(mName + "/OkButton", "OK", 60);
            mOk._assignListener(this);
            e = mOk.getOverlayElement();
            mDialogShade.AddChild(e);
            e.VerticalAlignment = GuiVerticalAlignment.GVA_CENTER;
            e.Left = -(e.Width / 2);
            e.Top = mDialog.getOverlayElement().Top + mDialog.getOverlayElement().Height + 5;
        }

        /*-----------------------------------------------------------------------------
        | Pops up a question dialog with Yes and No buttons.
        -----------------------------------------------------------------------------*/
        public void showYesNoDialog(string caption, string question) {
            if(mLoadBar != null) hideLoadingBar();

            OverlayElement e;

            if(mDialog != null) {
                mDialog.setCaption(caption);
                mDialog.setText(question);

                if(mOk != null) {
                    mOk.cleanup();
                    mOk = null;
                } else return;
            } else {
                // give widgets a chance to reset in case they're in the middle of something
                for(int i = 0; i < 10; i++) {
                    for(int j = 0; j < mWidgets[i].Count; j++)
                        mWidgets[i][j]._focusLost();
                }

                mDialogShade.Show();

                mDialog = new TextBox(mName + "/DialogBox", caption, 300, 208);
                mDialog.setText(question);
                e = mDialog.getOverlayElement();
                mDialogShade.AddChild(e);
                e.VerticalAlignment = GuiVerticalAlignment.GVA_CENTER;
                e.Left = -(e.Width / 2);
                e.Top = -(e.Height / 2);

                mCursorWasVisible = isCursorVisible();
                showCursor();
            }

            mYes = new Button(mName + "/YesButton", "Yes", 58);
            mYes._assignListener(this);
            e = mYes.getOverlayElement();
            mDialogShade.AddChild(e);
            e.VerticalAlignment = GuiVerticalAlignment.GVA_CENTER;
            e.Left = -(e.Width + 2);
            e.Top = mDialog.getOverlayElement().Top + mDialog.getOverlayElement().Height + 5;

            mNo = new Button(mName + "/NoButton", "No", 50);
            mNo._assignListener(this);
            e = mNo.getOverlayElement();
            mDialogShade.AddChild(e);
            e.VerticalAlignment = GuiVerticalAlignment.GVA_CENTER;
            e.Left = 3;
            e.Top = mDialog.getOverlayElement().Top + mDialog.getOverlayElement().Height + 5;
        }

        /*-----------------------------------------------------------------------------
        | Hides whatever dialog is currently showing.
        -----------------------------------------------------------------------------*/
        public void closeDialog() {
            if(mDialog != null) {
                if(mOk != null) {
                    mOk.cleanup();
                    mOk = null;
                } else {
                    mYes.cleanup();
                    mNo.cleanup();
                    mYes = null;
                    mNo = null;
                }

                mDialogShade.Hide();
                mDialog.cleanup();
                mDialog = null;

                if(!mCursorWasVisible) hideCursor();
            }
        }

        /*-----------------------------------------------------------------------------
        | Determines if any dialog is currently visible.
        -----------------------------------------------------------------------------*/
        public bool isDialogVisible() {
            return mDialog != null;
        }

        /*-----------------------------------------------------------------------------
        | Gets a widget from a tray by place.
        -----------------------------------------------------------------------------*/
        public Widget getWidget(TrayLocation trayLoc, int place) {
            return place < mWidgets[(int)trayLoc].Count ? mWidgets[(int)trayLoc][place] : null;
        }

        /*-----------------------------------------------------------------------------
        | Gets a widget from a tray by name.
        -----------------------------------------------------------------------------*/
        public Widget getWidget(TrayLocation trayLoc, string name) {
            for(int i = 0; i < mWidgets[(int)trayLoc].Count; i++)
                if(mWidgets[(int)trayLoc][i].getName() == name) { return mWidgets[(int)trayLoc][i]; }

            return null;
        }

        /*-----------------------------------------------------------------------------
        | Gets a widget by name.
        -----------------------------------------------------------------------------*/
        public Widget getWidget(string name) {
            for(int i = 0; i < 10; i++)
                for(int j = 0; j < mWidgets[i].Count; j++)
                    if(mWidgets[i][j].getName() == name) { return mWidgets[i][j]; }

            return null;
        }

        /*-----------------------------------------------------------------------------
        | Gets the number of widgets in total.
        -----------------------------------------------------------------------------*/
        public int getNumWidgets() {
            int total = 0;

            for(int i = 0; i < 10; i++)
                total += mWidgets[i].Count;

            return total;
        }

        /*-----------------------------------------------------------------------------
        | Gets the number of widgets in a tray.
        -----------------------------------------------------------------------------*/
        public int getNumWidgets(TrayLocation trayLoc) {
            return mWidgets[(int)trayLoc].Count;
        }

        /*-----------------------------------------------------------------------------
        | Gets a widget's position in its tray.
        -----------------------------------------------------------------------------*/
        public int locateWidgetInTray(Widget widget) {
            for(int i = 0; i < mWidgets[(int)widget.getTrayLocation()].Count; i++)
                if(mWidgets[(int)widget.getTrayLocation()][i] == widget) { return i; }

            return -1;
        }

        /*-----------------------------------------------------------------------------
        | Destroys a widget.
        -----------------------------------------------------------------------------*/
        public void destroyWidget(Widget widget) {
            if(widget == null) throw new Exception("Widget does not exist.");

            // in case special widgets are destroyed manually, set them to 0
            if(widget == mLogo) mLogo = null;
            else if(widget == mStatsPanel) mStatsPanel = null;
            else if(widget == mFpsLabel) mFpsLabel = null;
            
            // Not supposed to do the if
            if(widget.getName() != "") { mTrays[(int)widget.getTrayLocation()].RemoveChild(widget.getName()); }

            mWidgets[(int)widget.getTrayLocation()].Remove(widget);
            if(widget == mExpandedMenu) setExpandedMenu(null);

            widget.cleanup();
            mWidgetDeathRow.Add(widget);

            adjustTrays();
        }

        public void destroyWidget(TrayLocation trayLoc, int place) {
            destroyWidget(getWidget(trayLoc, place));
        }

        public void destroyWidget(TrayLocation trayLoc, string name) {
            destroyWidget(getWidget(trayLoc, name));
        }

        public void destroyWidget(string name) {
            destroyWidget(getWidget(name));
        }

        /*-----------------------------------------------------------------------------
        | Destroys all widgets in a tray.
        -----------------------------------------------------------------------------*/
        public void destroyAllWidgetsInTray(TrayLocation trayLoc) {
            while(mWidgets[(int)trayLoc].Count != 0) destroyWidget(mWidgets[(int)trayLoc][0]);
        }

        /*-----------------------------------------------------------------------------
        | Destroys all widgets.
        -----------------------------------------------------------------------------*/
        public void destroyAllWidgets() {
            for(int i = 0; i < 10; i++)  // destroy every widget in every tray (including null tray)
                destroyAllWidgetsInTray((TrayLocation)i);
        }

        /*-----------------------------------------------------------------------------
        | Adds a widget to a specified tray.
        -----------------------------------------------------------------------------*/
        public void moveWidgetToTray(Widget widget, TrayLocation trayLoc, int place = -1) {
            if(widget == null) throw new Exception("Widget does not exist.");

            // remove widget from old tray
            int it = mWidgets[(int)widget.getTrayLocation()].IndexOf(widget);
            if(it != mWidgets[(int)widget.getTrayLocation()].Count - 1 && it > 0) {
                mWidgets[(int)widget.getTrayLocation()].RemoveAt(it);
                mTrays[(int)widget.getTrayLocation()].RemoveChild(widget.getName());
            }

            // insert widget into new tray at given position, or at the end if unspecified or invalid
            if(place == -1 || place > mWidgets[(int)trayLoc].Count) place = mWidgets[(int)trayLoc].Count;
            mWidgets[(int)trayLoc].Insert(place, widget);
            mTrays[(int)trayLoc].AddChild(widget.getOverlayElement());

            widget.getOverlayElement().HorizontalAlignment = mTrayWidgetAlign[(int)trayLoc];

            // adjust trays if necessary
            if(widget.getTrayLocation() != TrayLocation.TL_NONE || trayLoc != TrayLocation.TL_NONE) adjustTrays();

            widget._assignToTray(trayLoc);
        }

        public void moveWidgetToTray(string name, TrayLocation trayLoc, int place = -1) {
            moveWidgetToTray(getWidget(name), trayLoc, place);
        }

        public void moveWidgetToTray(TrayLocation currentTrayLoc, string name, TrayLocation targetTrayLoc,
            int place = -1) {
            moveWidgetToTray(getWidget(currentTrayLoc, name), targetTrayLoc, place);
        }

        public void moveWidgetToTray(TrayLocation currentTrayLoc, int currentPlace, TrayLocation targetTrayLoc,
            int targetPlace = -1) {
            moveWidgetToTray(getWidget(currentTrayLoc, currentPlace), targetTrayLoc, targetPlace);
        }

        /*-----------------------------------------------------------------------------
        | Removes a widget from its tray. Same as moving it to the null tray.
        -----------------------------------------------------------------------------*/
        public void removeWidgetFromTray(Widget widget) {
            moveWidgetToTray(widget, TrayLocation.TL_NONE);
        }

        public void removeWidgetFromTray(string name) {
            removeWidgetFromTray(getWidget(name));
        }

        public void removeWidgetFromTray(TrayLocation trayLoc, string name) {
            removeWidgetFromTray(getWidget(trayLoc, name));
        }

        public void removeWidgetFromTray(TrayLocation trayLoc, int place) {
            removeWidgetFromTray(getWidget(trayLoc, place));
        }

        /*-----------------------------------------------------------------------------
        | Removes all widgets from a widget tray.
        -----------------------------------------------------------------------------*/
        public void clearTray(TrayLocation trayLoc) {
            if(trayLoc == TrayLocation.TL_NONE) return;      // can't clear the null tray

            while(mWidgets[(int)trayLoc].Count != 0)   // remove every widget from given tray
                removeWidgetFromTray(mWidgets[(int)trayLoc][0]);
        }

        /*-----------------------------------------------------------------------------
        | Removes all widgets from all widget trays.
        -----------------------------------------------------------------------------*/
        public void clearAllTrays() {
            for(int i = 0; i < 9; i++)
                clearTray((TrayLocation)i);
        }

        /*-----------------------------------------------------------------------------
        | Process frame events. Updates frame statistics widget set and deletes
        | all widgets queued for destruction.
        -----------------------------------------------------------------------------*/
        public bool frameRenderingQueued(FrameEvent evt) {
            for(int i = 0; i < mWidgetDeathRow.Count; i++)
                mWidgetDeathRow[i] = null;

            mWidgetDeathRow.Clear();

            if(areFrameStatsVisible()) {
                RenderTarget.FrameStats stats = mWindow.GetStatistics();
                mFpsLabel.setCaption("FPS: " + stats.LastFPS.ToString("N", CultureInfo.InvariantCulture));

                if(mStatsPanel.getOverlayElement().IsVisible) {
                    mStatsPanel.setAllParamValues(new string[]
				    {
				        stats.AvgFPS.ToString("N", CultureInfo.InvariantCulture),
				        stats.BestFPS.ToString("N", CultureInfo.InvariantCulture),
				        stats.WorstFPS.ToString("N", CultureInfo.InvariantCulture),
				        stats.TriangleCount.ToString("N", CultureInfo.InvariantCulture),
				        stats.BatchCount.ToString("N", CultureInfo.InvariantCulture)
				    });
                }
            }

            return true;
        }

        public void resourceGroupScriptingStarted(string groupName, int scriptCount) {
            mLoadInc = mGroupInitProportion / scriptCount;
            mLoadBar.setCaption("Parsing...");
            mWindow.Update();

        }

        public void scriptParseStarted(string scriptName, ref bool skipThisScript) {
            mLoadBar.setComment(scriptName);
            mWindow.Update();

        }

        public void scriptParseEnded(string scriptName, bool skipped) {
            mLoadBar.setProgress(mLoadBar.getProgress() + mLoadInc);
            mWindow.Update();

        }

        public void resourceGroupScriptingEnded(string groupName) { }

        public void resourceGroupLoadStarted(string groupName, int resourceCount) {
            mLoadInc = mGroupLoadProportion / resourceCount;
            mLoadBar.setCaption("Loading...");
            mWindow.Update();

        }

        public void resourceLoadStarted(ResourcePtr resource) {
            mLoadBar.setComment(resource.Name);
            mWindow.Update();
        }

        public void resourceLoadEnded() {
            mLoadBar.setProgress(mLoadBar.getProgress() + mLoadInc);
            mWindow.Update();
        }

        public void worldGeometryStageStarted(string description) {
            mLoadBar.setComment(description);
            mWindow.Update();
        }

        public void worldGeometryStageEnded() {
            mLoadBar.setProgress(mLoadBar.getProgress() + mLoadInc);
            mWindow.Update();
        }

        public void resourceGroupLoadEnded(string groupName) { }

        /*-----------------------------------------------------------------------------
        | Toggles visibility of advanced statistics.
        -----------------------------------------------------------------------------*/
        public new void labelHit(Label label) {
            if(mStatsPanel.getOverlayElement().IsVisible) {
                mStatsPanel.getOverlayElement().Hide();
                mFpsLabel.getOverlayElement().Width = 150;
                removeWidgetFromTray(mStatsPanel);
            } else {
                mStatsPanel.getOverlayElement().Show();
                mFpsLabel.getOverlayElement().Width = 180;
                moveWidgetToTray(mStatsPanel, mFpsLabel.getTrayLocation(), locateWidgetInTray(mFpsLabel) + 1);
            }
        }

        /*-----------------------------------------------------------------------------
        | Destroys dialog widgets, notifies listener, and ends high priority session.
        -----------------------------------------------------------------------------*/
        public new void buttonHit(Button button) {
            if(mListener != null) {
                if(button == mOk) mListener.okDialogClosed(mDialog.getText());
                else mListener.yesNoDialogClosed(mDialog.getText(), button == mYes);
            }
            closeDialog();
        }

        /*-----------------------------------------------------------------------------
        | Processes mouse button down events. Returns true if the event was
        | consumed and should not be passed on to other handlers.
        -----------------------------------------------------------------------------*/
        public bool injectMouseDown(MOIS.MouseEvent evt, MOIS.MouseButtonID id) {
            // only process left button when stuff is visible
            if(!mCursorLayer.IsVisible || id != MOIS.MouseButtonID.MB_Left) return false;
            Vector2 cursorPos = new Vector2(mCursor.Left, mCursor.Top);

            mTrayDrag = false;

            if(mExpandedMenu != null)   // only check top priority widget until it passes on
			{
                mExpandedMenu._cursorPressed(cursorPos);
                if(!mExpandedMenu.isExpanded()) setExpandedMenu(null);
                return true;
            }

            if(mDialog != null)   // only check top priority widget until it passes on
			{
                mDialog._cursorPressed(cursorPos);
                if(mOk != null) mOk._cursorPressed(cursorPos);
                else {
                    mYes._cursorPressed(cursorPos);
                    mNo._cursorPressed(cursorPos);
                }
                return true;
            }

            for(int i = 0; i < 9; i++)   // check if mouse is over a non-null tray
			{
                if(mTrays[i].IsVisible && Widget.isCursorOver(mTrays[i], cursorPos, 2)) {
                    mTrayDrag = true;   // initiate a drag that originates in a tray
                    break;
                }
            }

            for(int i = 0; i < mWidgets[9].Count; i++)  // check if mouse is over a non-null tray's widgets
			{
                if(mWidgets[9][i].getOverlayElement().IsVisible &&
                    Widget.isCursorOver(mWidgets[9][i].getOverlayElement(), cursorPos)) {
                    mTrayDrag = true;   // initiate a drag that originates in a tray
                    break;
                }
            }

            if(!mTrayDrag) return false;   // don't process if mouse press is not in tray

            for(int i = 0; i < 10; i++) {
                if(!mTrays[i].IsVisible) continue;

                for(int j = 0; j < mWidgets[i].Count; j++) {
                    Widget w = mWidgets[i][j];
                    if(!w.getOverlayElement().IsVisible) continue;
                    w._cursorPressed(cursorPos);    // send event to widget

                    if (w.GetType().IsInstanceOfType(typeof (SelectMenu)))
                    {
                        SelectMenu m = (SelectMenu) w;
                        if (m != null && m.isExpanded()) // a menu has begun a top priority session
                        {
                            setExpandedMenu(m);
                            return true;
                        }
                    }
                }
            }

            return true;   // a tray click is not to be handled by another party
        }

        /*-----------------------------------------------------------------------------
        | Processes mouse button up events. Returns true if the event was
        | consumed and should not be passed on to other handlers.
        -----------------------------------------------------------------------------*/
        public bool injectMouseUp(OIS.MouseEvent evt, MOIS.MouseButtonID id) {
            // only process left button when stuff is visible
            if(!mCursorLayer.IsVisible || id != MOIS.MouseButtonID.MB_Left) return false;
            Vector2 cursorPos = new Vector2(mCursor.Left, mCursor.Top);

            if(mExpandedMenu != null)   // only check top priority widget until it passes on
			{
                mExpandedMenu._cursorReleased(cursorPos);
                return true;
            }

            if(mDialog != null)   // only check top priority widget until it passes on
			{
                mDialog._cursorReleased(cursorPos);
                if(mOk != null) mOk._cursorReleased(cursorPos);
                else {
                    mYes._cursorReleased(cursorPos);
                    // very important to check if second button still exists, because first button could've closed the popup
                    if(mNo != null) mNo._cursorReleased(cursorPos);
                }
                return true;
            }

            if(!mTrayDrag) return false;    // this click did not originate in a tray, so don't process

            Widget w;

            for(int i = 0; i < 10; i++) {
                if(!mTrays[i].IsVisible) continue;

                for(int j = 0; j < mWidgets[i].Count; j++) {
                    w = mWidgets[i][j];
                    if(!w.getOverlayElement().IsVisible) continue;
                    w._cursorReleased(cursorPos);    // send event to widget
                }
            }

            mTrayDrag = false;   // stop this drag
            return true;         // this click did originate in this tray, so don't pass it on
        }

        /*-----------------------------------------------------------------------------
        | Updates cursor position. Returns true if the event was
        | consumed and should not be passed on to other handlers.
        -----------------------------------------------------------------------------*/
        public bool injectMouseMove(MOIS.MouseEvent evt) {
            if(!mCursorLayer.IsVisible) return false;   // don't process if cursor layer is invisible

            Vector2 cursorPos = new Vector2(evt.state.X.abs, evt.state.Y.abs);
            mCursor.SetPosition(cursorPos.x, cursorPos.y);

            if(mExpandedMenu != null)   // only check top priority widget until it passes on
			{
                mExpandedMenu._cursorMoved(cursorPos);
                return true;
            }

            if(mDialog != null)   // only check top priority widget until it passes on
			{
                mDialog._cursorMoved(cursorPos);
                if(mOk != null) mOk._cursorMoved(cursorPos);
                else {
                    mYes._cursorMoved(cursorPos);
                    mNo._cursorMoved(cursorPos);
                }
                return true;
            }

            Widget w;

            for(int i = 0; i < 10; i++) {
                if(!mTrays[i].IsVisible) continue;

                for(int j = 0; j < mWidgets[i].Count; j++) {
                    w = mWidgets[i][j];
                    if(!w.getOverlayElement().IsVisible) continue;
                    w._cursorMoved(cursorPos);    // send event to widget
                }
            }

            return mTrayDrag;
        }

        /*-----------------------------------------------------------------------------
        | Internal method to prioritise / deprioritise expanded menus.
        -----------------------------------------------------------------------------*/
        protected void setExpandedMenu(SelectMenu m) {
            if(mExpandedMenu == null && m != null) {
                OverlayContainer c = (OverlayContainer)m.getOverlayElement();
                OverlayContainer eb = (OverlayContainer)c.GetChild(m.getName() + "/MenuExpandedBox");
                eb._update();
                eb.SetPosition
                    ((int)(eb._getDerivedLeft() * OverlayManager.Singleton.ViewportWidth),
                    (int)(eb._getDerivedTop() * OverlayManager.Singleton.ViewportHeight));
                c.RemoveChild(eb.Name);
                mPriorityLayer.Add2D(eb);
            } else if(mExpandedMenu != null && m != null) {
                OverlayContainer eb = mPriorityLayer.GetChild(mExpandedMenu.getName() + "/MenuExpandedBox");
                mPriorityLayer.Remove2D(eb);
                ((OverlayContainer)mExpandedMenu.getOverlayElement()).AddChild(eb);
            }

            mExpandedMenu = m;
        }
    }
}
