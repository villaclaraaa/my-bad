using System.Text;

namespace Mybad.Core.Services;

/// <summary>
/// Defines a contract for sending notification messages.
/// </summary>
public interface INotifier
{
	/// <summary>
	/// Send Notification Message.
	/// </summary>
	/// <param name="message">Instance of <see cref="NotifyMessage"/>. Contains at least some text.</param>
	/// <returns>Task representing asynchronous operation.</returns>
	Task NotifyAsync(NotifyMessage message);
}

/// <summary>
/// Represents a notification message that includes a main text and optional additional information.
/// </summary>
/// <param name="Text">The main text content of the notification message. Cannot be null.</param>
/// <param name="AdditionalInfo">An optional array of additional information objects to include with the message. May be null if no extra information
/// is provided.</param>
/// <remarks>It implements <b>ToString()</b> method override with basic ToString() method on all <see cref="AdditionalInfo"/> objects. Be careful.</remarks>
public record class NotifyMessage(string Text, object[]? AdditionalInfo = null)
{
	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.Append(Text);
		if (AdditionalInfo != null)
		{
			foreach (var info in AdditionalInfo)
			{
				sb.Append(info.ToString() + Environment.NewLine);
			}
		}

		return sb.ToString();
	}
}