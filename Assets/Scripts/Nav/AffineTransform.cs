using Pika.Ai.Nav;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text;

namespace Pika.Ai.Nav
{


    /**
     * The <code>AffineTransform</code> class represents a 2D affine transform
     * that performs a linear mapping from 2D coordinates to other 2D
     * coordinates that preserves the "straightness" and
     * "parallelness" of lines.  Affine transformations can be constructed
     * using sequences of translations, scales, flips, rotations, and shears.
     * <p>
     * Such a coordinate transformation can be represented by a 3 row by
     * 3 column matrix with an implied last row of [ 0 0 1 ].  This matrix
     * transforms source coordinates {@code (x,y)} into
     * destination coordinates {@code (x',y')} by considering
     * them to be a column vector and multiplying the coordinate vector
     * by the matrix according to the following process:
     * <pre>
     *      [ x']   [  m00  m01  m02  ] [ x ]   [ m00x + m01y + m02 ]
     *      [ y'] = [  m10  m11  m12  ] [ y ] = [ m10x + m11y + m12 ]
     *      [ 1 ]   [   0    0    1   ] [ 1 ]   [         1         ]
     * </pre>
     * <p>
     * <a name="quadrantapproximation"><h4>Handling 90-Degree Rotations</h4></a>
     * <p>
     * In some variations of the <code>rotate</code> methods in the
     * <code>AffineTransform</code> class, a double-precision argument
     * specifies the angle of rotation in radians.
     * These methods have special handling for rotations of approximately
     * 90 degrees (including multiples such as 180, 270, and 360 degrees),
     * so that the common case of quadrant rotation is handled more
     * efficiently.
     * This special handling can cause angles very close to multiples of
     * 90 degrees to be treated as if they were exact multiples of
     * 90 degrees.
     * For small multiples of 90 degrees the range of angles treated
     * as a quadrant rotation is approximately 0.00000121 degrees wide.
     * This section explains why such special care is needed and how
     * it is implemented.
     * <p>
     * Since 90 degrees is represented as <code>PI/2</code> in radians,
     * and since PI is a transcendental (and therefore irrational) number,
     * it is not possible to exactly represent a multiple of 90 degrees as
     * an exact double precision value measured in radians.
     * As a result it is theoretically impossible to describe quadrant
     * rotations (90, 180, 270 or 360 degrees) using these values.
     * Double precision floating point values can get very close to
     * non-zero multiples of <code>PI/2</code> but never close enough
     * for the sine or cosine to be exactly 0.0, 1.0 or -1.0.
     * The implementations of <code>Math.sin()</code> and
     * <code>Math.cos()</code> correspondingly never return 0.0
     * for any case other than <code>Math.sin(0.0)</code>.
     * These same implementations do, however, return exactly 1.0 and
     * -1.0 for some range of numbers around each multiple of 90
     * degrees since the correct answer is so close to 1.0 or -1.0 that
     * the double precision significand cannot represent the difference
     * as accurately as it can for numbers that are near 0.0.
     * <p>
     * The net result of these issues is that if the
     * <code>Math.sin()</code> and <code>Math.cos()</code> methods
     * are used to directly generate the values for the matrix modifications
     * during these radian-based rotation operations then the resulting
     * transform is never strictly classifiable as a quadrant rotation
     * even for a simple case like <code>rotate(Math.PI/2.0)</code>,
     * due to minor variations in the matrix caused by the non-0.0 values
     * obtained for the sine and cosine.
     * If these transforms are not classified as quadrant rotations then
     * subsequent code which attempts to optimize further operations based
     * upon the type of the transform will be relegated to its most general
     * implementation.
     * <p>
     * Because quadrant rotations are fairly common,
     * this class should handle these cases reasonably quickly, both in
     * applying the rotations to the transform and in applying the resulting
     * transform to the coordinates.
     * To facilitate this optimal handling, the methods which take an angle
     * of rotation measured in radians attempt to detect angles that are
     * intended to be quadrant rotations and treat them as such.
     * These methods therefore treat an angle <em>theta</em> as a quadrant
     * rotation if either <code>Math.sin(<em>theta</em>)</code> or
     * <code>Math.cos(<em>theta</em>)</code> returns exactly 1.0 or -1.0.
     * As a rule of thumb, this property holds true for a range of
     * approximately 0.0000000211 radians (or 0.00000121 degrees) around
     * small multiples of <code>Math.PI/2.0</code>.
     *
     * @author Jim Graham
     * @since 1.2
     */
    public class AffineTransform
    {

        /*
         * This constant is only useful for the cached type field.
         * It indicates that the type has been decached and must be recalculated.
         */
        private const int TYPE_UNKNOWN = -1;

        /**
         * This constant indicates that the transform defined by this object
         * is an identity transform.
         * An identity transform is one in which the output coordinates are
         * always the same as the input coordinates.
         * If this transform is anything other than the identity transform,
         * the type will either be the constant GENERAL_TRANSFORM or a
         * combination of the appropriate flag bits for the various coordinate
         * conversions that this transform performs.
         * @see #TYPE_TRANSLATION
         * @see #TYPE_UNIFORM_SCALE
         * @see #TYPE_GENERAL_SCALE
         * @see #TYPE_FLIP
         * @see #TYPE_QUADRANT_ROTATION
         * @see #TYPE_GENERAL_ROTATION
         * @see #TYPE_GENERAL_TRANSFORM
         * @see #getType
         * @since 1.2
         */
        public const int TYPE_IDENTITY = 0;

        /**
         * This flag bit indicates that the transform defined by this object
         * performs a translation in addition to the conversions indicated
         * by other flag bits.
         * A translation moves the coordinates by a constant amount in x
         * and y without changing the length or angle of vectors.
         * @see #TYPE_IDENTITY
         * @see #TYPE_UNIFORM_SCALE
         * @see #TYPE_GENERAL_SCALE
         * @see #TYPE_FLIP
         * @see #TYPE_QUADRANT_ROTATION
         * @see #TYPE_GENERAL_ROTATION
         * @see #TYPE_GENERAL_TRANSFORM
         * @see #getType
         * @since 1.2
         */
        public const int TYPE_TRANSLATION = 1;

        /**
         * This flag bit indicates that the transform defined by this object
         * performs a uniform scale in addition to the conversions indicated
         * by other flag bits.
         * A uniform scale multiplies the length of vectors by the same amount
         * in both the x and y directions without changing the angle between
         * vectors.
         * This flag bit is mutually exclusive with the TYPE_GENERAL_SCALE flag.
         * @see #TYPE_IDENTITY
         * @see #TYPE_TRANSLATION
         * @see #TYPE_GENERAL_SCALE
         * @see #TYPE_FLIP
         * @see #TYPE_QUADRANT_ROTATION
         * @see #TYPE_GENERAL_ROTATION
         * @see #TYPE_GENERAL_TRANSFORM
         * @see #getType
         * @since 1.2
         */
        public const int TYPE_UNIFORM_SCALE = 2;

        /**
         * This flag bit indicates that the transform defined by this object
         * performs a general scale in addition to the conversions indicated
         * by other flag bits.
         * A general scale multiplies the length of vectors by different
         * amounts in the x and y directions without changing the angle
         * between perpendicular vectors.
         * This flag bit is mutually exclusive with the TYPE_UNIFORM_SCALE flag.
         * @see #TYPE_IDENTITY
         * @see #TYPE_TRANSLATION
         * @see #TYPE_UNIFORM_SCALE
         * @see #TYPE_FLIP
         * @see #TYPE_QUADRANT_ROTATION
         * @see #TYPE_GENERAL_ROTATION
         * @see #TYPE_GENERAL_TRANSFORM
         * @see #getType
         * @since 1.2
         */
        public const int TYPE_GENERAL_SCALE = 4;

        /**
         * This constant is a bit mask for any of the scale flag bits.
         * @see #TYPE_UNIFORM_SCALE
         * @see #TYPE_GENERAL_SCALE
         * @since 1.2
         */
        public const int TYPE_MASK_SCALE = (TYPE_UNIFORM_SCALE |
                                                   TYPE_GENERAL_SCALE);

        /**
         * This flag bit indicates that the transform defined by this object
         * performs a mirror image flip about some axis which changes the
         * normally right handed coordinate system into a left handed
         * system in addition to the conversions indicated by other flag bits.
         * A right handed coordinate system is one where the positive X
         * axis rotates counterclockwise to overlay the positive Y axis
         * similar to the direction that the fingers on your right hand
         * curl when you stare end on at your thumb.
         * A left handed coordinate system is one where the positive X
         * axis rotates clockwise to overlay the positive Y axis similar
         * to the direction that the fingers on your left hand curl.
         * There is no mathematical way to determine the angle of the
         * original flipping or mirroring transformation since all angles
         * of flip are identical given an appropriate adjusting rotation.
         * @see #TYPE_IDENTITY
         * @see #TYPE_TRANSLATION
         * @see #TYPE_UNIFORM_SCALE
         * @see #TYPE_GENERAL_SCALE
         * @see #TYPE_QUADRANT_ROTATION
         * @see #TYPE_GENERAL_ROTATION
         * @see #TYPE_GENERAL_TRANSFORM
         * @see #getType
         * @since 1.2
         */
        public const int TYPE_FLIP = 64;
        /* NOTE: TYPE_FLIP was added after GENERAL_TRANSFORM was in public
         * circulation and the flag bits could no longer be conveniently
         * renumbered without introducing binary incompatibility in outside
         * code.
         */

        /**
         * This flag bit indicates that the transform defined by this object
         * performs a quadrant rotation by some multiple of 90 degrees in
         * addition to the conversions indicated by other flag bits.
         * A rotation changes the angles of vectors by the same amount
         * regardless of the original direction of the vector and without
         * changing the length of the vector.
         * This flag bit is mutually exclusive with the TYPE_GENERAL_ROTATION flag.
         * @see #TYPE_IDENTITY
         * @see #TYPE_TRANSLATION
         * @see #TYPE_UNIFORM_SCALE
         * @see #TYPE_GENERAL_SCALE
         * @see #TYPE_FLIP
         * @see #TYPE_GENERAL_ROTATION
         * @see #TYPE_GENERAL_TRANSFORM
         * @see #getType
         * @since 1.2
         */
        public const int TYPE_QUADRANT_ROTATION = 8;

        /**
         * This flag bit indicates that the transform defined by this object
         * performs a rotation by an arbitrary angle in addition to the
         * conversions indicated by other flag bits.
         * A rotation changes the angles of vectors by the same amount
         * regardless of the original direction of the vector and without
         * changing the length of the vector.
         * This flag bit is mutually exclusive with the
         * TYPE_QUADRANT_ROTATION flag.
         * @see #TYPE_IDENTITY
         * @see #TYPE_TRANSLATION
         * @see #TYPE_UNIFORM_SCALE
         * @see #TYPE_GENERAL_SCALE
         * @see #TYPE_FLIP
         * @see #TYPE_QUADRANT_ROTATION
         * @see #TYPE_GENERAL_TRANSFORM
         * @see #getType
         * @since 1.2
         */
        public const int TYPE_GENERAL_ROTATION = 16;

        /**
         * This constant is a bit mask for any of the rotation flag bits.
         * @see #TYPE_QUADRANT_ROTATION
         * @see #TYPE_GENERAL_ROTATION
         * @since 1.2
         */
        public const int TYPE_MASK_ROTATION = (TYPE_QUADRANT_ROTATION |
                                                      TYPE_GENERAL_ROTATION);

        /**
         * This constant indicates that the transform defined by this object
         * performs an arbitrary conversion of the input coordinates.
         * If this transform can be classified by any of the above constants,
         * the type will either be the constant TYPE_IDENTITY or a
         * combination of the appropriate flag bits for the various coordinate
         * conversions that this transform performs.
         * @see #TYPE_IDENTITY
         * @see #TYPE_TRANSLATION
         * @see #TYPE_UNIFORM_SCALE
         * @see #TYPE_GENERAL_SCALE
         * @see #TYPE_FLIP
         * @see #TYPE_QUADRANT_ROTATION
         * @see #TYPE_GENERAL_ROTATION
         * @see #getType
         * @since 1.2
         */
        public const int TYPE_GENERAL_TRANSFORM = 32;

        /**
         * This constant is used for the internal state variable to indicate
         * that no calculations need to be performed and that the source
         * coordinates only need to be copied to their destinations to
         * complete the transformation equation of this transform.
         * @see #APPLY_TRANSLATE
         * @see #APPLY_SCALE
         * @see #APPLY_SHEAR
         * @see #state
         */
        const int APPLY_IDENTITY = 0;

        /**
         * This constant is used for the internal state variable to indicate
         * that the translation components of the matrix (m02 and m12) need
         * to be added to complete the transformation equation of this transform.
         * @see #APPLY_IDENTITY
         * @see #APPLY_SCALE
         * @see #APPLY_SHEAR
         * @see #state
         */
        const int APPLY_TRANSLATE = 1;

        /**
         * This constant is used for the internal state variable to indicate
         * that the scaling components of the matrix (m00 and m11) need
         * to be factored in to complete the transformation equation of
         * this transform.  If the APPLY_SHEAR bit is also set then it
         * indicates that the scaling components are not both 0.0.  If the
         * APPLY_SHEAR bit is not also set then it indicates that the
         * scaling components are not both 1.0.  If neither the APPLY_SHEAR
         * nor the APPLY_SCALE bits are set then the scaling components
         * are both 1.0, which means that the x and y components contribute
         * to the transformed coordinate, but they are not multiplied by
         * any scaling factor.
         * @see #APPLY_IDENTITY
         * @see #APPLY_TRANSLATE
         * @see #APPLY_SHEAR
         * @see #state
         */
        const int APPLY_SCALE = 2;

        /**
         * This constant is used for the internal state variable to indicate
         * that the shearing components of the matrix (m01 and m10) need
         * to be factored in to complete the transformation equation of this
         * transform.  The presence of this bit in the state variable changes
         * the interpretation of the APPLY_SCALE bit as indicated in its
         * documentation.
         * @see #APPLY_IDENTITY
         * @see #APPLY_TRANSLATE
         * @see #APPLY_SCALE
         * @see #state
         */
        const int APPLY_SHEAR = 4;

        /*
         * For methods which combine together the state of two separate
         * transforms and dispatch based upon the combination, these constants
         * specify how far to shift one of the states so that the two states
         * are mutually non-interfering and provide constants for testing the
         * bits of the shifted (HI) state.  The methods in this class use
         * the convention that the state of "this" transform is unshifted and
         * the state of the "other" or "argument" transform is shifted (HI).
         */
        private const int HI_SHIFT = 3;
        private const int HI_IDENTITY = APPLY_IDENTITY << HI_SHIFT;
        private const int HI_TRANSLATE = APPLY_TRANSLATE << HI_SHIFT;
        private const int HI_SCALE = APPLY_SCALE << HI_SHIFT;
        private const int HI_SHEAR = APPLY_SHEAR << HI_SHIFT;

        /**
         * The X coordinate scaling element of the 3x3
         * affine transformation matrix.
         *
         * @serial
         */
        double m00;

        /**
         * The Y coordinate shearing element of the 3x3
         * affine transformation matrix.
         *
         * @serial
         */
        double m10;

        /**
         * The X coordinate shearing element of the 3x3
         * affine transformation matrix.
         *
         * @serial
         */
        double m01;

        /**
         * The Y coordinate scaling element of the 3x3
         * affine transformation matrix.
         *
         * @serial
         */
        double m11;

        /**
         * The X coordinate of the translation element of the
         * 3x3 affine transformation matrix.
         *
         * @serial
         */
        double m02;

        /**
         * The Y coordinate of the translation element of the
         * 3x3 affine transformation matrix.
         *
         * @serial
         */
        double m12;

        /**
         * This field keeps track of which components of the matrix need to
         * be applied when performing a transformation.
         * @see #APPLY_IDENTITY
         * @see #APPLY_TRANSLATE
         * @see #APPLY_SCALE
         * @see #APPLY_SHEAR
         */
        int state;

        /**
         * This field caches the current transformation type of the matrix.
         * @see #TYPE_IDENTITY
         * @see #TYPE_TRANSLATION
         * @see #TYPE_UNIFORM_SCALE
         * @see #TYPE_GENERAL_SCALE
         * @see #TYPE_FLIP
         * @see #TYPE_QUADRANT_ROTATION
         * @see #TYPE_GENERAL_ROTATION
         * @see #TYPE_GENERAL_TRANSFORM
         * @see #TYPE_UNKNOWN
         * @see #getType
         */
        private int type;

        private AffineTransform(double m00, double m10,
                                double m01, double m11,
                                double m02, double m12,
                                int state)
        {
            this.m00 = m00;
            this.m10 = m10;
            this.m01 = m01;
            this.m11 = m11;
            this.m02 = m02;
            this.m12 = m12;
            this.state = state;
            this.type = TYPE_UNKNOWN;
        }

        /**
         * Constructs a new <code>AffineTransform</code> representing the
         * Identity transformation.
         * @since 1.2
         */
        public AffineTransform()
        {
            m00 = m11 = 1.0;
            // m01 = m10 = m02 = m12 = 0.0;         /* Not needed. */
            // state = APPLY_IDENTITY;              /* Not needed. */
            // type = TYPE_IDENTITY;                /* Not needed. */
        }

        /**
         * Constructs a new <code>AffineTransform</code> that is a copy of
         * the specified <code>AffineTransform</code> object.
         * @param Tx the <code>AffineTransform</code> object to copy
         * @since 1.2
         */
        public AffineTransform(AffineTransform Tx)
        {
            this.m00 = Tx.m00;
            this.m10 = Tx.m10;
            this.m01 = Tx.m01;
            this.m11 = Tx.m11;
            this.m02 = Tx.m02;
            this.m12 = Tx.m12;
            this.state = Tx.state;
            this.type = Tx.type;
        }

        /**
         * Constructs a new <code>AffineTransform</code> from 6 floating point
         * values representing the 6 specifiable entries of the 3x3
         * transformation matrix.
         *
         * @param m00 the X coordinate scaling element of the 3x3 matrix
         * @param m10 the Y coordinate shearing element of the 3x3 matrix
         * @param m01 the X coordinate shearing element of the 3x3 matrix
         * @param m11 the Y coordinate scaling element of the 3x3 matrix
         * @param m02 the X coordinate translation element of the 3x3 matrix
         * @param m12 the Y coordinate translation element of the 3x3 matrix
         * @since 1.2
         */

        public AffineTransform(float m00, float m10,
                               float m01, float m11,
                               float m02, float m12)
        {
            this.m00 = m00;
            this.m10 = m10;
            this.m01 = m01;
            this.m11 = m11;
            this.m02 = m02;
            this.m12 = m12;
            updateState();
        }

        /**
         * Constructs a new <code>AffineTransform</code> from an array of
         * floating point values representing either the 4 non-translation
         * enries or the 6 specifiable entries of the 3x3 transformation
         * matrix.  The values are retrieved from the array as
         * {&nbsp;m00&nbsp;m10&nbsp;m01&nbsp;m11&nbsp;[m02&nbsp;m12]}.
         * @param flatmatrix the float array containing the values to be set
         * in the new <code>AffineTransform</code> object. The length of the
         * array is assumed to be at least 4. If the length of the array is
         * less than 6, only the first 4 values are taken. If the length of
         * the array is greater than 6, the first 6 values are taken.
         * @since 1.2
         */
        public AffineTransform(float[] flatmatrix)
        {
            m00 = flatmatrix[0];
            m10 = flatmatrix[1];
            m01 = flatmatrix[2];
            m11 = flatmatrix[3];
            if (flatmatrix.Length > 5)
            {
                m02 = flatmatrix[4];
                m12 = flatmatrix[5];
            }
            updateState();
        }

        /**
         * Constructs a new <code>AffineTransform</code> from 6 double
         * precision values representing the 6 specifiable entries of the 3x3
         * transformation matrix.
         *
         * @param m00 the X coordinate scaling element of the 3x3 matrix
         * @param m10 the Y coordinate shearing element of the 3x3 matrix
         * @param m01 the X coordinate shearing element of the 3x3 matrix
         * @param m11 the Y coordinate scaling element of the 3x3 matrix
         * @param m02 the X coordinate translation element of the 3x3 matrix
         * @param m12 the Y coordinate translation element of the 3x3 matrix
         * @since 1.2
         */
        public AffineTransform(double m00, double m10,
                               double m01, double m11,
                               double m02, double m12)
        {
            this.m00 = m00;
            this.m10 = m10;
            this.m01 = m01;
            this.m11 = m11;
            this.m02 = m02;
            this.m12 = m12;
            updateState();
        }

        /**
         * Constructs a new <code>AffineTransform</code> from an array of
         * double precision values representing either the 4 non-translation
         * entries or the 6 specifiable entries of the 3x3 transformation
         * matrix. The values are retrieved from the array as
         * {&nbsp;m00&nbsp;m10&nbsp;m01&nbsp;m11&nbsp;[m02&nbsp;m12]}.
         * @param flatmatrix the double array containing the values to be set
         * in the new <code>AffineTransform</code> object. The length of the
         * array is assumed to be at least 4. If the length of the array is
         * less than 6, only the first 4 values are taken. If the length of
         * the array is greater than 6, the first 6 values are taken.
         * @since 1.2
         */
        public AffineTransform(double[] flatmatrix)
        {
            m00 = flatmatrix[0];
            m10 = flatmatrix[1];
            m01 = flatmatrix[2];
            m11 = flatmatrix[3];
            if (flatmatrix.Length > 5)
            {
                m02 = flatmatrix[4];
                m12 = flatmatrix[5];
            }
            updateState();
        }



        /**
         * Retrieves the flag bits describing the conversion properties of
         * this transform.
         * The return value is either one of the constants TYPE_IDENTITY
         * or TYPE_GENERAL_TRANSFORM, or a combination of the
         * appriopriate flag bits.
         * A valid combination of flag bits is an exclusive OR operation
         * that can combine
         * the TYPE_TRANSLATION flag bit
         * in addition to either of the
         * TYPE_UNIFORM_SCALE or TYPE_GENERAL_SCALE flag bits
         * as well as either of the
         * TYPE_QUADRANT_ROTATION or TYPE_GENERAL_ROTATION flag bits.
         * @return the OR combination of any of the indicated flags that
         * apply to this transform
         * @see #TYPE_IDENTITY
         * @see #TYPE_TRANSLATION
         * @see #TYPE_UNIFORM_SCALE
         * @see #TYPE_GENERAL_SCALE
         * @see #TYPE_QUADRANT_ROTATION
         * @see #TYPE_GENERAL_ROTATION
         * @see #TYPE_GENERAL_TRANSFORM
         * @since 1.2
         */
        public int getType()
        {
            if (type == TYPE_UNKNOWN)
            {
                calculateType();
            }
            return type;
        }

        /**
         * This is the utility function to calculate the flag bits when
         * they have not been cached.
         * @see #getType
         */
        private void calculateType()
        {
            int ret = TYPE_IDENTITY;
            bool sgn0, sgn1;
            double M0, M1, M2, M3;
            updateState();
            switch (state)
            {

                /* NOTREACHED */
                case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
                    ret = TYPE_TRANSLATION;
                    if ((M0 = m00) * (M2 = m01) + (M3 = m10) * (M1 = m11) != 0)
                    {
                        // Transformed unit vectors are not perpendicular...
                        this.type = TYPE_GENERAL_TRANSFORM;
                        return;
                    }
                    sgn0 = (M0 >= 0.0);
                    sgn1 = (M1 >= 0.0);
                    if (sgn0 == sgn1)
                    {
                        // sgn(M0) == sgn(M1) therefore sgn(M2) == -sgn(M3)
                        // This is the "unflipped" (right-handed) state
                        if (M0 != M1 || M2 != -M3)
                        {
                            ret |= (TYPE_GENERAL_ROTATION | TYPE_GENERAL_SCALE);
                        }
                        else if (M0 * M1 - M2 * M3 != 1.0)
                        {
                            ret |= (TYPE_GENERAL_ROTATION | TYPE_UNIFORM_SCALE);
                        }
                        else
                        {
                            ret |= TYPE_GENERAL_ROTATION;
                        }
                    }
                    else
                    {
                        // sgn(M0) == -sgn(M1) therefore sgn(M2) == sgn(M3)
                        // This is the "flipped" (left-handed) state
                        if (M0 != -M1 || M2 != M3)
                        {
                            ret |= (TYPE_GENERAL_ROTATION |
                                    TYPE_FLIP |
                                    TYPE_GENERAL_SCALE);
                        }
                        else if (M0 * M1 - M2 * M3 != 1.0)
                        {
                            ret |= (TYPE_GENERAL_ROTATION |
                                    TYPE_FLIP |
                                    TYPE_UNIFORM_SCALE);
                        }
                        else
                        {
                            ret |= (TYPE_GENERAL_ROTATION | TYPE_FLIP);
                        }
                    }
                    break;
                /* NOBREAK */
                case (APPLY_SHEAR | APPLY_SCALE):
                    if ((M0 = m00) * (M2 = m01) + (M3 = m10) * (M1 = m11) != 0)
                    {
                        // Transformed unit vectors are not perpendicular...
                        this.type = TYPE_GENERAL_TRANSFORM;
                        return;
                    }
                    sgn0 = (M0 >= 0.0);
                    sgn1 = (M1 >= 0.0);
                    if (sgn0 == sgn1)
                    {
                        // sgn(M0) == sgn(M1) therefore sgn(M2) == -sgn(M3)
                        // This is the "unflipped" (right-handed) state
                        if (M0 != M1 || M2 != -M3)
                        {
                            ret |= (TYPE_GENERAL_ROTATION | TYPE_GENERAL_SCALE);
                        }
                        else if (M0 * M1 - M2 * M3 != 1.0)
                        {
                            ret |= (TYPE_GENERAL_ROTATION | TYPE_UNIFORM_SCALE);
                        }
                        else
                        {
                            ret |= TYPE_GENERAL_ROTATION;
                        }
                    }
                    else
                    {
                        // sgn(M0) == -sgn(M1) therefore sgn(M2) == sgn(M3)
                        // This is the "flipped" (left-handed) state
                        if (M0 != -M1 || M2 != M3)
                        {
                            ret |= (TYPE_GENERAL_ROTATION |
                                    TYPE_FLIP |
                                    TYPE_GENERAL_SCALE);
                        }
                        else if (M0 * M1 - M2 * M3 != 1.0)
                        {
                            ret |= (TYPE_GENERAL_ROTATION |
                                    TYPE_FLIP |
                                    TYPE_UNIFORM_SCALE);
                        }
                        else
                        {
                            ret |= (TYPE_GENERAL_ROTATION | TYPE_FLIP);
                        }
                    }
                    break;
                case (APPLY_SHEAR | APPLY_TRANSLATE):
                    ret = TYPE_TRANSLATION;
                    sgn0 = ((M0 = m01) >= 0.0);
                    sgn1 = ((M1 = m10) >= 0.0);
                    if (sgn0 != sgn1)
                    {
                        // Different signs - simple 90 degree rotation
                        if (M0 != -M1)
                        {
                            ret |= (TYPE_QUADRANT_ROTATION | TYPE_GENERAL_SCALE);
                        }
                        else if (M0 != 1.0 && M0 != -1.0)
                        {
                            ret |= (TYPE_QUADRANT_ROTATION | TYPE_UNIFORM_SCALE);
                        }
                        else
                        {
                            ret |= TYPE_QUADRANT_ROTATION;
                        }
                    }
                    else
                    {
                        // Same signs - 90 degree rotation plus an axis flip too
                        if (M0 == M1)
                        {
                            ret |= (TYPE_QUADRANT_ROTATION |
                                    TYPE_FLIP |
                                    TYPE_UNIFORM_SCALE);
                        }
                        else
                        {
                            ret |= (TYPE_QUADRANT_ROTATION |
                                    TYPE_FLIP |
                                    TYPE_GENERAL_SCALE);
                        }
                    }
                    break;
                /* NOBREAK */
                case (APPLY_SHEAR):
                    sgn0 = ((M0 = m01) >= 0.0);
                    sgn1 = ((M1 = m10) >= 0.0);
                    if (sgn0 != sgn1)
                    {
                        // Different signs - simple 90 degree rotation
                        if (M0 != -M1)
                        {
                            ret |= (TYPE_QUADRANT_ROTATION | TYPE_GENERAL_SCALE);
                        }
                        else if (M0 != 1.0 && M0 != -1.0)
                        {
                            ret |= (TYPE_QUADRANT_ROTATION | TYPE_UNIFORM_SCALE);
                        }
                        else
                        {
                            ret |= TYPE_QUADRANT_ROTATION;
                        }
                    }
                    else
                    {
                        // Same signs - 90 degree rotation plus an axis flip too
                        if (M0 == M1)
                        {
                            ret |= (TYPE_QUADRANT_ROTATION |
                                    TYPE_FLIP |
                                    TYPE_UNIFORM_SCALE);
                        }
                        else
                        {
                            ret |= (TYPE_QUADRANT_ROTATION |
                                    TYPE_FLIP |
                                    TYPE_GENERAL_SCALE);
                        }
                    }
                    break;
                case (APPLY_SCALE | APPLY_TRANSLATE):
                    ret = TYPE_TRANSLATION;
                    sgn0 = ((M0 = m00) >= 0.0);
                    sgn1 = ((M1 = m11) >= 0.0);
                    if (sgn0 == sgn1)
                    {
                        if (sgn0)
                        {
                            // Both scaling factors non-negative - simple scale
                            // Note: APPLY_SCALE implies M0, M1 are not both 1
                            if (M0 == M1)
                            {
                                ret |= TYPE_UNIFORM_SCALE;
                            }
                            else
                            {
                                ret |= TYPE_GENERAL_SCALE;
                            }
                        }
                        else
                        {
                            // Both scaling factors negative - 180 degree rotation
                            if (M0 != M1)
                            {
                                ret |= (TYPE_QUADRANT_ROTATION | TYPE_GENERAL_SCALE);
                            }
                            else if (M0 != -1.0)
                            {
                                ret |= (TYPE_QUADRANT_ROTATION | TYPE_UNIFORM_SCALE);
                            }
                            else
                            {
                                ret |= TYPE_QUADRANT_ROTATION;
                            }
                        }
                    }
                    else
                    {
                        // Scaling factor signs different - flip about some axis
                        if (M0 == -M1)
                        {
                            if (M0 == 1.0 || M0 == -1.0)
                            {
                                ret |= TYPE_FLIP;
                            }
                            else
                            {
                                ret |= (TYPE_FLIP | TYPE_UNIFORM_SCALE);
                            }
                        }
                        else
                        {
                            ret |= (TYPE_FLIP | TYPE_GENERAL_SCALE);
                        }
                    }
                    break;
                /* NOBREAK */
                case (APPLY_SCALE):
                    sgn0 = ((M0 = m00) >= 0.0);
                    sgn1 = ((M1 = m11) >= 0.0);
                    if (sgn0 == sgn1)
                    {
                        if (sgn0)
                        {
                            // Both scaling factors non-negative - simple scale
                            // Note: APPLY_SCALE implies M0, M1 are not both 1
                            if (M0 == M1)
                            {
                                ret |= TYPE_UNIFORM_SCALE;
                            }
                            else
                            {
                                ret |= TYPE_GENERAL_SCALE;
                            }
                        }
                        else
                        {
                            // Both scaling factors negative - 180 degree rotation
                            if (M0 != M1)
                            {
                                ret |= (TYPE_QUADRANT_ROTATION | TYPE_GENERAL_SCALE);
                            }
                            else if (M0 != -1.0)
                            {
                                ret |= (TYPE_QUADRANT_ROTATION | TYPE_UNIFORM_SCALE);
                            }
                            else
                            {
                                ret |= TYPE_QUADRANT_ROTATION;
                            }
                        }
                    }
                    else
                    {
                        // Scaling factor signs different - flip about some axis
                        if (M0 == -M1)
                        {
                            if (M0 == 1.0 || M0 == -1.0)
                            {
                                ret |= TYPE_FLIP;
                            }
                            else
                            {
                                ret |= (TYPE_FLIP | TYPE_UNIFORM_SCALE);
                            }
                        }
                        else
                        {
                            ret |= (TYPE_FLIP | TYPE_GENERAL_SCALE);
                        }
                    }
                    break;
                case (APPLY_TRANSLATE):
                    ret = TYPE_TRANSLATION;
                    break;
                case (APPLY_IDENTITY):
                    break;
                default:
                    stateError();
                    break;
            }
            this.type = ret;
        }

        /**
         * Returns the determinant of the matrix representation of the transform.
         * The determinant is useful both to determine if the transform can
         * be inverted and to get a single value representing the
         * combined X and Y scaling of the transform.
         * <p>
         * If the determinant is non-zero, then this transform is
         * invertible and the various methods that depend on the inverse
         * transform do not need to throw a
         * {@link NoninvertibleTransformException}.
         * If the determinant is zero then this transform can not be
         * inverted since the transform maps all input coordinates onto
         * a line or a point.
         * If the determinant is near enough to zero then inverse transform
         * operations might not carry enough precision to produce meaningful
         * results.
         * <p>
         * If this transform represents a uniform scale, as indicated by
         * the <code>getType</code> method then the determinant also
         * represents the square of the uniform scale factor by which all of
         * the points are expanded from or contracted towards the origin.
         * If this transform represents a non-uniform scale or more general
         * transform then the determinant is not likely to represent a
         * value useful for any purpose other than determining if inverse
         * transforms are possible.
         * <p>
         * Mathematically, the determinant is calculated using the formula:
         * <pre>
         *          |  m00  m01  m02  |
         *          |  m10  m11  m12  |  =  m00 * m11 - m01 * m10
         *          |   0    0    1   |
         * </pre>
         *
         * @return the determinant of the matrix used to transform the
         * coordinates.
         * @see #getType
         * @see #createInverse
         * @see #inverseTransform
         * @see #TYPE_UNIFORM_SCALE
         * @since 1.2
         */
        public double getDeterminant()
        {
            switch (state)
            {

                /* NOTREACHED */
                case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
                case (APPLY_SHEAR | APPLY_SCALE):
                    return m00 * m11 - m01 * m10;
                case (APPLY_SHEAR | APPLY_TRANSLATE):
                case (APPLY_SHEAR):
                    return -(m01 * m10);
                case (APPLY_SCALE | APPLY_TRANSLATE):
                case (APPLY_SCALE):
                    return m00 * m11;
                case (APPLY_TRANSLATE):
                case (APPLY_IDENTITY):
                    return 1.0;
                default:
                    stateError();
                    return 0.0f;
            }
        }

        /**
         * Manually recalculates the state of the transform when the matrix
         * changes too much to predict the effects on the state.
         * The following table specifies what the various settings of the
         * state field say about the values of the corresponding matrix
         * element fields.
         * Note that the rules governing the SCALE fields are slightly
         * different depending on whether the SHEAR flag is also set.
         * <pre>
         *                     SCALE            SHEAR          TRANSLATE
         *                    m00/m11          m01/m10          m02/m12
         *
         * IDENTITY             1.0              0.0              0.0
         * TRANSLATE (TR)       1.0              0.0          not both 0.0
         * SCALE (SC)       not both 1.0         0.0              0.0
         * TR | SC          not both 1.0         0.0          not both 0.0
         * SHEAR (SH)           0.0          not both 0.0         0.0
         * TR | SH              0.0          not both 0.0     not both 0.0
         * SC | SH          not both 0.0     not both 0.0         0.0
         * TR | SC | SH     not both 0.0     not both 0.0     not both 0.0
         * </pre>
         */
        void updateState()
        {
            if (m01 == 0.0 && m10 == 0.0)
            {
                if (m00 == 1.0 && m11 == 1.0)
                {
                    if (m02 == 0.0 && m12 == 0.0)
                    {
                        state = APPLY_IDENTITY;
                        type = TYPE_IDENTITY;
                    }
                    else
                    {
                        state = APPLY_TRANSLATE;
                        type = TYPE_TRANSLATION;
                    }
                }
                else
                {
                    if (m02 == 0.0 && m12 == 0.0)
                    {
                        state = APPLY_SCALE;
                        type = TYPE_UNKNOWN;
                    }
                    else
                    {
                        state = (APPLY_SCALE | APPLY_TRANSLATE);
                        type = TYPE_UNKNOWN;
                    }
                }
            }
            else
            {
                if (m00 == 0.0 && m11 == 0.0)
                {
                    if (m02 == 0.0 && m12 == 0.0)
                    {
                        state = APPLY_SHEAR;
                        type = TYPE_UNKNOWN;
                    }
                    else
                    {
                        state = (APPLY_SHEAR | APPLY_TRANSLATE);
                        type = TYPE_UNKNOWN;
                    }
                }
                else
                {
                    if (m02 == 0.0 && m12 == 0.0)
                    {
                        state = (APPLY_SHEAR | APPLY_SCALE);
                        type = TYPE_UNKNOWN;
                    }
                    else
                    {
                        state = (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE);
                        type = TYPE_UNKNOWN;
                    }
                }
            }
        }

        /*
         * Convenience method used internally to throw exceptions when
         * a case was forgotten in a switch statement.
         */
        private void stateError()
        {
            throw new Exception("missing case in transform state switch");
        }



        /**
         * Transforms an array of floating point coordinates by this transform.
         * The two coordinate array sections can be exactly the same or
         * can be overlapping sections of the same array without affecting the
         * validity of the results.
         * This method ensures that no source coordinates are overwritten by a
         * previous operation before they can be transformed.
         * The coordinates are stored in the arrays starting at the specified
         * offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>.
         * @param srcPts the array containing the source point coordinates.
         * Each point is stored as a pair of x,&nbsp;y coordinates.
         * @param dstPts the array into which the transformed point coordinates
         * are returned.  Each point is stored as a pair of x,&nbsp;y
         * coordinates.
         * @param srcOff the offset to the first point to be transformed
         * in the source array
         * @param dstOff the offset to the location of the first
         * transformed point that is stored in the destination array
         * @param numPts the number of points to be transformed
         * @since 1.2
         */
        public void transform(float[] srcPts, int srcOff,
                              float[] dstPts, int dstOff,
                              int numPts)
        {
            double M00, M01, M02, M10, M11, M12;    // For caching
            if (dstPts == srcPts &&
                dstOff > srcOff && dstOff < srcOff + numPts * 2)
            {
                // If the arrays overlap partially with the destination higher
                // than the source and we transform the coordinates normally
                // we would overwrite some of the later source coordinates
                // with results of previous transformations.
                // To get around this we use arraycopy to copy the points
                // to their final destination with correct overwrite
                // handling and then transform them in place in the new
                // safer location.
                Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2);
                // srcPts = dstPts;         // They are known to be equal.
                srcOff = dstOff;
            }
            switch (state)
            {

                /* NOTREACHED */
                case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
                    M00 = m00; M01 = m01; M02 = m02;
                    M10 = m10; M11 = m11; M12 = m12;
                    while (--numPts >= 0)
                    {
                        double x = srcPts[srcOff++];
                        double y = srcPts[srcOff++];
                        dstPts[dstOff++] = (float)(M00 * x + M01 * y + M02);
                        dstPts[dstOff++] = (float)(M10 * x + M11 * y + M12);
                    }
                    return;
                case (APPLY_SHEAR | APPLY_SCALE):
                    M00 = m00; M01 = m01;
                    M10 = m10; M11 = m11;
                    while (--numPts >= 0)
                    {
                        double x = srcPts[srcOff++];
                        double y = srcPts[srcOff++];
                        dstPts[dstOff++] = (float)(M00 * x + M01 * y);
                        dstPts[dstOff++] = (float)(M10 * x + M11 * y);
                    }
                    return;
                case (APPLY_SHEAR | APPLY_TRANSLATE):
                    M01 = m01; M02 = m02;
                    M10 = m10; M12 = m12;
                    while (--numPts >= 0)
                    {
                        double x = srcPts[srcOff++];
                        dstPts[dstOff++] = (float)(M01 * srcPts[srcOff++] + M02);
                        dstPts[dstOff++] = (float)(M10 * x + M12);
                    }
                    return;
                case (APPLY_SHEAR):
                    M01 = m01; M10 = m10;
                    while (--numPts >= 0)
                    {
                        double x = srcPts[srcOff++];
                        dstPts[dstOff++] = (float)(M01 * srcPts[srcOff++]);
                        dstPts[dstOff++] = (float)(M10 * x);
                    }
                    return;
                case (APPLY_SCALE | APPLY_TRANSLATE):
                    M00 = m00; M02 = m02;
                    M11 = m11; M12 = m12;
                    while (--numPts >= 0)
                    {
                        dstPts[dstOff++] = (float)(M00 * srcPts[srcOff++] + M02);
                        dstPts[dstOff++] = (float)(M11 * srcPts[srcOff++] + M12);
                    }
                    return;
                case (APPLY_SCALE):
                    M00 = m00; M11 = m11;
                    while (--numPts >= 0)
                    {
                        dstPts[dstOff++] = (float)(M00 * srcPts[srcOff++]);
                        dstPts[dstOff++] = (float)(M11 * srcPts[srcOff++]);
                    }
                    return;
                case (APPLY_TRANSLATE):
                    M02 = m02; M12 = m12;
                    while (--numPts >= 0)
                    {
                        dstPts[dstOff++] = (float)(srcPts[srcOff++] + M02);
                        dstPts[dstOff++] = (float)(srcPts[srcOff++] + M12);
                    }
                    return;
                case (APPLY_IDENTITY):
                    if (srcPts != dstPts || srcOff != dstOff)
                    {
                        Array.Copy(srcPts, srcOff, dstPts, dstOff,
                                         numPts * 2);
                    }
                    return;
                default:
                    stateError();
                    return;
            }

            /* NOTREACHED */
        }

    }
}


public class NoninvertibleTransformException : System.Exception
{
    public NoninvertibleTransformException(string msg) : base(msg) { }
}