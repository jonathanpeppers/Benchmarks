// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Shared;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.Tasks
{
	/// <summary>
	/// Generates a hash of a given ItemGroup items. Metadata is not considered in the hash.
	/// <remarks>
	/// Currently uses SHA1. Implementation subject to change between MSBuild versions. Not
	/// intended as a cryptographic security measure, only uniqueness between build executions.
	/// </remarks>
	/// </summary>
	public class Hash2 : Task
	{
		/// <summary>
		/// Result of "\u2028" passed to Encoding.UTF8.GetBytes()
		/// </summary>
		private static readonly byte [] ItemSeparatorCharacter = new byte [] { 0xe2, 0x80, 0xa8 };

		/// <summary>
		/// Items from which to generate a hash.
		/// </summary>
		[Required]
		public ITaskItem [] ItemsToHash { get; set; }

		/// <summary>
		/// When true, will generate a case-insensitive hash.
		/// </summary>
		public bool IgnoreCase { get; set; }

		/// <summary>
		/// Hash of the ItemsToHash ItemSpec.
		/// </summary>
		[Output]
		public string HashResult { get; set; }

		/// <summary>
		/// Execute the task.
		/// </summary>
		public override bool Execute ()
		{
			if (ItemsToHash != null && ItemsToHash.Length > 0) {
				using (var sha1 = SHA1.Create ()) {
					var encoding = Encoding.UTF8;
					var maxItemStringSize = ComputeMaxStringSize (encoding, ItemsToHash);
					var bytes = new byte [maxItemStringSize];
					foreach (var item in ItemsToHash) {
						string itemSpec = IgnoreCase ? item.ItemSpec.ToUpperInvariant () : item.ItemSpec;
						int length = encoding.GetBytes (itemSpec, 0, itemSpec.Length, bytes, 0);
						sha1.TransformBlock (bytes, 0, length, null, 0);
						sha1.TransformBlock (ItemSeparatorCharacter, 0, ItemSeparatorCharacter.Length, null, 0);
					}

					sha1.TransformFinalBlock (Array.Empty<byte> (), 0, 0);
					using (var stringBuilder = new ReuseableStringBuilder (sha1.HashSize)) {
						foreach (var b in sha1.Hash) {
							stringBuilder.Append (b.ToString ("x2"));
						}
						HashResult = stringBuilder.ToString ();
					}
				}
			}

			return true;
		}

		private int ComputeMaxStringSize (Encoding encoding, ITaskItem [] itemsToHash)
		{
			var maxSize = 0;
			foreach (var item in itemsToHash) {
				var length = encoding.GetByteCount (item.ItemSpec);
				if (length > maxSize) {
					maxSize = length;
				}
			}

			return maxSize;
		}
	}
}
